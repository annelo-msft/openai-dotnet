﻿using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace OpenAI.Examples;

public partial class AssistantExamples
{
    [Test]
    public void Example02_FunctionCalling()
    {
        #region Define Functions
        string GetCurrentLocation()
        {
            // Call a location API here.
            return "San Francisco";
        }

        const string GetCurrentLocationFunctionName = "get_current_location";

        FunctionToolDefinition getLocationTool = new()
        {
            FunctionName = GetCurrentLocationFunctionName,
            Description = "Get the user's current location"
        };

        string GetCurrentWeather(string location, string unit = "celsius")
        {
            // Call a weather API here.
            return $"31 {unit}";
        }

        const string GetCurrentWeatherFunctionName = "get_current_weather";

        FunctionToolDefinition getWeatherTool = new()
        {
            FunctionName = GetCurrentWeatherFunctionName,
            Description = "Get the current weather in a given location",
            Parameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The city and state, e.g. Boston, MA"
                    },
                    "unit": {
                        "type": "string",
                        "enum": [ "celsius", "fahrenheit" ],
                        "description": "The temperature unit to use. Infer this from the specified location."
                    }
                },
                "required": [ "location" ]
            }
            """),
        };
        #endregion

        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
#pragma warning disable OPENAI001
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region Create Assistant
        // Create an assistant that can call the function tools.
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Example: Function Calling",
            Instructions =
                "Don't make assumptions about what values to plug into functions."
                + " Ask for clarification if a user request is ambiguous.",
            Tools = { getLocationTool, getWeatherTool },
        };

        Assistant assistant = client.CreateAssistant("gpt-4-turbo", assistantOptions);
        #endregion

        #region Create Thread and Run
        // Create a thread with an initial user message and run it.
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { "What's the weather like today?" }
        };

        RunOperation runOperation = client.CreateThreadAndRun(ReturnWhen.Started, assistant.Id, threadOptions);
        #endregion

        #region Submit tool outputs to run

        IEnumerable<ThreadRun> updates = runOperation.GetUpdates();

        foreach (ThreadRun update in updates)
        {
            if (update.Status == RunStatus.RequiresAction)
            {
                List<ToolOutput> toolOutputs = [];

                foreach (RequiredAction action in runOperation.Value.RequiredActions)
                {
                    switch (action.FunctionName)
                    {
                        case GetCurrentLocationFunctionName:
                            {
                                string toolResult = GetCurrentLocation();
                                toolOutputs.Add(new ToolOutput(action.ToolCallId, toolResult));
                                break;
                            }

                        case GetCurrentWeatherFunctionName:
                            {
                                // The arguments that the model wants to use to call the function are specified as a
                                // stringified JSON object based on the schema defined in the tool definition. Note that
                                // the model may hallucinate arguments too. Consequently, it is important to do the
                                // appropriate parsing and validation before calling the function.
                                using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
                                bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                if (!hasLocation)
                                {
                                    throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                }

                                string toolResult = hasUnit
                                    ? GetCurrentWeather(location.GetString(), unit.GetString())
                                    : GetCurrentWeather(location.GetString());
                                toolOutputs.Add(new ToolOutput(action.ToolCallId, toolResult));
                                break;
                            }

                        default:
                            {
                                // Handle other or unexpected calls.
                                throw new NotImplementedException();
                            }
                    }
                }

                // Submit the tool outputs to the assistant, which returns the run to the queued state.
                runOperation.SubmitToolOutputsToRun(toolOutputs);
            }
        }

        #endregion

        #region Get and display messages

        // If the run completed successfully, list the messages and display their content
        if (runOperation.Status == RunStatus.Completed)
        {
            PageCollection<ThreadMessage> messagePages
                = client.GetMessages(runOperation.ThreadId, new MessageCollectionOptions() { Order = ListOrder.OldestFirst });
            IEnumerable<ThreadMessage> messages = messagePages.GetAllValues();

            foreach (ThreadMessage message in messages)
            {
                Console.WriteLine($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    Console.WriteLine($"{contentItem.Text}");

                    if (contentItem.ImageFileId is not null)
                    {
                        Console.WriteLine($" <Image File ID> {contentItem.ImageFileId}");
                    }

                    // Include annotations, if any.
                    if (contentItem.TextAnnotations.Count > 0)
                    {
                        Console.WriteLine();
                        foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                        {
                            Console.WriteLine($"* File ID used by file_search: {annotation.InputFileId}");
                            Console.WriteLine($"* File ID created by code_interpreter: {annotation.OutputFileId}");
                            Console.WriteLine($"* Text to replace: {annotation.TextToReplace}");
                            Console.WriteLine($"* Message content index range: {annotation.StartIndex}-{annotation.EndIndex}");
                        }
                    }

                }
                Console.WriteLine();
            }
        }
        else
        {
            throw new NotImplementedException(runOperation.Status.ToString());
        }
        #endregion
    }
}
