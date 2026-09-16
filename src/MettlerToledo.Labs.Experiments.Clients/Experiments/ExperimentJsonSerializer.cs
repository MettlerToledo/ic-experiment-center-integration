// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// A class to help with serializing the <see cref="ExperimentJson"/> and trend values.
/// </summary>
public static class ExperimentJsonSerializer
{
    /// <summary>
    /// The serializer options for serializing ExperimentJson.
    /// </summary>
    public static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();
    /// <summary>
    /// Serializer options that skip reading the trend values, but reads the other content of ExperimentJson.
    /// </summary>
    public static readonly JsonSerializerOptions SerializerOptionsDoNotReadTrendValues = CreateSerializerOptionsDoNotReadTrendValues();

    /// <summary>
    /// Reads an ExperimentJson from the stream without loading the trend values.
    /// </summary>
    public static async Task<ExperimentJson?> DeserializeWithoutTrendValues(Stream stream)
    {
        return await JsonSerializer.DeserializeAsync<ExperimentJson>(stream, SerializerOptionsDoNotReadTrendValues);
    }

    /// <summary>
    /// Deserialize strem to StageWithMaterialsJson without loading the trend values
    /// </summary>
    public static async Task<StageWithMaterialsJson?> DeserializeStageWithoutTrendValues(Stream stream)
    {
        return await JsonSerializer.DeserializeAsync<StageWithMaterialsJson>(stream, SerializerOptionsDoNotReadTrendValues);
    }

    /// <summary>
    /// Reads a trend from a stream containing an ExperimentJson.
    /// </summary>
    public static async Task ReadTrends(Stream stream, Func<TrendPathJson, TrendJson, Task> processTrend)
    {
        var trendReader = new TrendReader(stream, processTrend);
        await trendReader.ReadTrends();
    }

    /// <summary>
    /// Serializes the ExperimentJson to the stream while getting the trend data from a separate source one trend at a time.
    /// </summary>
    public static async Task Serialize(Stream stream, ExperimentJson experimentJson, Func<TrendPathJson, Task<IEnumerable<TrendValueJson>>> trendDataProvider)
    {
        var experimentTrendWriter = new ExperimentTrendWriter(stream, trendDataProvider);

        await experimentTrendWriter.Serialize(experimentJson);
    }

    /// <summary>
    /// Serializes the StageWithMaterialsJson to the stream while getting the trend data from a separate source one trend at a time.
    /// </summary>
    public static async Task SerializeStage(Stream stream, StageWithMaterialsJson stageWithMaterialsJson, Func<TrendPathJson, Task<IEnumerable<TrendValueJson>>> trendDataProvider)
    {
        var experimentTrendWriter = new ExperimentTrendWriter(stream, trendDataProvider);

        await experimentTrendWriter.SeralizeStage(stageWithMaterialsJson);
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        serializerOptions.MakeReadOnly();
        return serializerOptions;
    }

    private static JsonSerializerOptions CreateSerializerOptionsDoNotReadTrendValues()
    {
        var serializerOptions = new JsonSerializerOptions(SerializerOptions);
        serializerOptions.Converters.Add(new TrendJsonNoValuesReadConverter());
        serializerOptions.MakeReadOnly();
        return serializerOptions;
    }

    private class ExperimentTrendWriter
    {
        private readonly Utf8JsonWriter _jsonWriter;
        private readonly Func<TrendPathJson, Task<IEnumerable<TrendValueJson>>> _trendValuesProvider;
        private Guid _currentStage;
        private Guid _currentEquipment;
        private Guid _currentOccupation;

        public ExperimentTrendWriter(Stream stream, Func<TrendPathJson, Task<IEnumerable<TrendValueJson>>> trendValuesProvider)
        {
            _jsonWriter = new Utf8JsonWriter(stream);
            _trendValuesProvider = trendValuesProvider;
        }

        public async Task Serialize(ExperimentJson experimentJson)
        {
            await WriteExperiment(experimentJson);

            await _jsonWriter.FlushAsync();
        }

        public async Task SeralizeStage(StageWithMaterialsJson stageWithMaterialsJson)
        {
            await WriteStageWithMaterialsJson(stageWithMaterialsJson);

            await _jsonWriter.FlushAsync();
        }

        private async Task WriteExperiment(ExperimentJson experimentJson)
        {
            await WriteObject(experimentJson, typeof(ExperimentJson).GetProperties(), nameof(ExperimentJson.Process),
                () => WriteProcess(experimentJson.Process));
        }

        private async Task WriteStageWithMaterialsJson(StageWithMaterialsJson stageWithMaterialsJson)
        {
            await WriteObject(stageWithMaterialsJson, typeof(StageWithMaterialsJson).GetProperties(), nameof(StageWithMaterialsJson.Stage),
                () => WriteStage(stageWithMaterialsJson.Stage));
        }

        private async Task WriteProcess(ProcessJson process)
        {
            await WriteObject(process, typeof(ProcessJson).GetProperties(), nameof(ProcessJson.Stages),
                () => WriteCollection(process.Stages, WriteStage));
        }

        private async Task WriteStage(StageJson stage)
        {
            _currentStage = stage.TrackingId;

            await WriteObject(stage, typeof(StageJson).GetProperties(), nameof(StageJson.EquipmentCollection),
                () => WriteCollection(stage.EquipmentCollection, WriteEquipment));
        }

        private async Task WriteEquipment(EquipmentJson equipment)
        {
            _currentEquipment = equipment.TrackingId;

            await WriteObject(equipment, equipment.GetType().GetProperties(), nameof(InstrumentJson.Zones),
                () => WriteCollection(((InstrumentJson)equipment).Zones, WriteInstrumentZone), true);
        }

        private async Task WriteInstrumentZone(InstrumentZoneJson instrumentZone)
        {
            await WriteObject(instrumentZone, instrumentZone.GetType().GetProperties(), nameof(InstrumentZoneJson.Occupations),
                () => WriteCollection(instrumentZone.Occupations, WriteOccupation), true);
        }

        private async Task WriteOccupation(OccupationJson occupation)
        {
            _currentOccupation = occupation.TrackingId;

            await WriteObject(occupation, typeof(OccupationJson).GetProperties(), nameof(OccupationJson.Trends),
                () => WriteTrends(occupation.Trends));
        }

        private async Task WriteObject<T>(T jsonObject, IReadOnlyCollection<PropertyInfo> properties, string specialPropertyName,
            Func<Task> writeSpecialProperty, bool writeTypeProperty = false)
            where T : class
        {
            _jsonWriter.WriteStartObject();
            if (writeTypeProperty)
            {
                WriteTypeProperty(jsonObject);
            }

            foreach (var property in properties)
            {
                _jsonWriter.WritePropertyName(ToCamelCase(property.Name));
                if (property.Name != specialPropertyName)
                {
                    WriteNormalPropertyValue(jsonObject, property);
                }
                else
                {
                    await writeSpecialProperty();
                }
            }
            _jsonWriter.WriteEndObject();
        }

        private async Task WriteTrends(IList<TrendJson> trends)
        {
            _jsonWriter.WriteStartArray();
            foreach (var trend in trends)
            {
                var values = await _trendValuesProvider(new TrendPathJson(_currentStage, _currentEquipment, _currentOccupation, trend.Name));
                TrendJsonNoValuesReadConverter.Write(_jsonWriter, trend, values, SerializerOptions);
            }
            _jsonWriter.WriteEndArray();
        }

        private async Task WriteCollection<T>(IList<T> collection, Func<T, Task> writeItem)
        {
            _jsonWriter.WriteStartArray();

            foreach (var item in collection)
            {
                await writeItem(item);
            }

            _jsonWriter.WriteEndArray();
        }

        private void WriteTypeProperty(object jsonObject)
        {
            var typeName = jsonObject.GetType().Name;

            _jsonWriter.WriteString("$type", typeName.Substring(0, typeName.Length - 4));
        }

        private void WriteNormalPropertyValue(object jsonObject, PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                _jsonWriter.WriteStringValue((string)property.GetValue(jsonObject));
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                _jsonWriter.WriteStringValue((DateTime)property.GetValue(jsonObject));
            }
            else
            {
                JsonSerializer.Serialize(_jsonWriter, property.GetValue(jsonObject), property.PropertyType, SerializerOptions);
            }
        }

        private static string ToCamelCase(string name)
        {
            return $"{(char)(name[0] + ('a' - 'A'))}{name.Substring(1)}";
        }
    }

    private class TrendReader
    {
        private enum ReadingState { Initial, GettingToProcess, InProcess, InStages, InEquipment, InZones, InOccupations, InTrends, ReadingTrend, HaveTrend, ReadToEnd, End }

        private const int BufferSize = 65_536;
        private const int HalfBufferSize = BufferSize / 2;
        private readonly Stream _stream;
        private readonly Func<TrendPathJson, TrendJson, Task> _processTrend;
        private readonly Stack<JsonTokenType> _startTokensStack = new Stack<JsonTokenType>();
        private ReadingState _state;
        private JsonReaderState _readerState;
        private TrendJson? _trendJson;
        private Guid _stageId;
        private Guid _equipmentId;
        private Guid _occupationId;
        private bool _singleStage = false;

        public TrendReader(Stream stream, Func<TrendPathJson, TrendJson, Task> processTrend)
        {
            _stream = stream;
            _processTrend = processTrend;
            _state = ReadingState.Initial;
        }

        public async Task ReadTrends()
        {
            var buffer = new byte[BufferSize];
            var bufferLength = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var positionInBuffer = 0;

            while (_state != ReadingState.End)
            {
                if (_state == ReadingState.HaveTrend)
                {
                    if (_trendJson is null)
                    {
                        throw new InvalidOperationException($"We should have a trend in state {ReadingState.HaveTrend}");
                    }
                    var trendPath = new TrendPathJson(_stageId, _equipmentId, _occupationId, _trendJson.Name);
                    await _processTrend(trendPath, _trendJson);
                    _state = ReadingState.InTrends;
                }

                var remainingBytesInBuffer = bufferLength - positionInBuffer;
                if (remainingBytesInBuffer < HalfBufferSize)
                {
                    if (positionInBuffer > 0 && remainingBytesInBuffer > 0)
                    {
                        // TODO: Can this result in overlap in copy.
                        Array.Copy(buffer, positionInBuffer, buffer, 0, bufferLength - positionInBuffer);
                    }
                    positionInBuffer = 0;
                    bufferLength = remainingBytesInBuffer;
                    bufferLength += await _stream.ReadAsync(buffer, bufferLength, BufferSize - bufferLength);
                    remainingBytesInBuffer = bufferLength;
                }

                positionInBuffer += ReadToken(new ReadOnlySpan<byte>(buffer, positionInBuffer, remainingBytesInBuffer));
            }
        }

        private int ReadToken(ReadOnlySpan<byte> buffer)
        {
            var jsonReader = new Utf8JsonReader(buffer, false, _readerState);
            if (jsonReader.Read())
            {
                switch (_state)
                {
                    case ReadingState.Initial:
                        if (jsonReader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException("Expected start of object");
                        }
                        _state = ReadingState.GettingToProcess;
                        break;
                    case ReadingState.GettingToProcess:
                        GetToProcessOrSingleStage(ref jsonReader);
                        break;
                    case ReadingState.InProcess:
                        ReadProcess(ref jsonReader);
                        break;
                    case ReadingState.InStages:
                        if (_singleStage)
                        {
                            ReadSingleStage(ref jsonReader);
                        }
                        else
                        {
                            ReadObjectInArray(ref jsonReader, "equipmentCollection", ReadingState.InEquipment, ReadingState.InProcess, 0);
                        }
                        break;
                    case ReadingState.InEquipment:
                        var equipmentStackLevel = _singleStage ? 0 : 1;
                        ReadObjectInArray(ref jsonReader, "zones", ReadingState.InZones, ReadingState.InStages, equipmentStackLevel);
                        break;
                    case ReadingState.InZones:
                        var zoneStackLevel = _singleStage ? 1 : 2;
                        ReadObjectInArray(ref jsonReader, "occupations", ReadingState.InOccupations, ReadingState.InEquipment, zoneStackLevel);
                        break;
                    case ReadingState.InOccupations:
                        var occupationStackLevel = _singleStage ? 2 : 3;
                        ReadObjectInArray(ref jsonReader, "trends", ReadingState.InTrends, ReadingState.InZones, occupationStackLevel);
                        break;
                    case ReadingState.InTrends:
                        ReadTrend(ref jsonReader);
                        break;
                    case ReadingState.ReadingTrend:
                        ReadTrendValues(ref jsonReader);
                        break;
                    case ReadingState.ReadToEnd:
                        ReadToObjectEnd(ref jsonReader, ReadingState.End);
                        break;
                    default:
                        throw new NotImplementedException($"{_state} not implemented.");
                }
            }
            else
            {
                throw new JsonException("Unexpected end of document or large than expected token.");
            }
            _readerState = jsonReader.CurrentState;
            return (int)jsonReader.BytesConsumed; // Our buffer is not big enough for int to be an issue here.
        }

        private void GetToProcessOrSingleStage(ref Utf8JsonReader jsonReader)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    ReadToObjectEnd(ref jsonReader, ReadingState.End);
                    break;
                case JsonTokenType.PropertyName:
                    // We can get to Occupations in two different ways depending on the object
                    // If we find process then we are parsing an ExperimentJson and need to parse a list of stages
                    // If we find stage then we are parsing a single stage in a StageWithMaterialsJson object

                    if (_startTokensStack.Count == 0 && jsonReader.ValueTextEquals("process"))
                    {
                        jsonReader.Read();
                        if (jsonReader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException("Expected start object.");
                        }
                        _state = ReadingState.InProcess;
                    }
                    else if (_startTokensStack.Count == 0 && jsonReader.ValueTextEquals("stage"))
                    {
                        jsonReader.Read();
                        if (jsonReader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException("Expected start object.");
                        }
                        _singleStage = true;
                        _state = ReadingState.InStages;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReadProcess(ref Utf8JsonReader jsonReader)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    ReadToObjectEnd(ref jsonReader, ReadingState.ReadToEnd);
                    break;
                case JsonTokenType.PropertyName:
                    if (_startTokensStack.Count == 0 && jsonReader.ValueTextEquals("stages"))
                    {
                        jsonReader.Read();
                        if (jsonReader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException("Expected start array.");
                        }
                        _state = ReadingState.InStages;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReadSingleStage(ref Utf8JsonReader jsonReader)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    ReadToObjectEnd(ref jsonReader, ReadingState.ReadToEnd);
                    break;
                case JsonTokenType.PropertyName:
                    if (_startTokensStack.Count == 0 && jsonReader.ValueTextEquals("equipmentCollection"))
                    {
                        jsonReader.Read();
                        if (jsonReader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException("Expected start array.");
                        }
                        _state = ReadingState.InEquipment;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReadObjectInArray(ref Utf8JsonReader jsonReader, string searchProperty, ReadingState propertyState, ReadingState endState,
            int baseStackLevel)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    ReadToArrayEnd(ref jsonReader, endState, baseStackLevel);
                    break;
                case JsonTokenType.PropertyName:
                    if (_startTokensStack.Count == baseStackLevel + 1) // The objects will be at one level more inside the array.
                    {
                        if (jsonReader.ValueTextEquals(searchProperty))
                        {
                            jsonReader.Read();
                            if (jsonReader.TokenType != JsonTokenType.StartArray)
                            {
                                throw new JsonException("Expected start array.");
                            }
                            _state = propertyState;
                        }
                        else if (jsonReader.ValueTextEquals("trackingId"))
                        {
                            HandleTrackingId(ref jsonReader);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleTrackingId(ref Utf8JsonReader jsonReader)
        {
            jsonReader.Read();
            if (jsonReader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var trackingId = jsonReader.GetGuid();
            switch (_state)
            {
                case ReadingState.InStages:
                    _stageId = trackingId;
                    break;
                case ReadingState.InEquipment:
                    _equipmentId = trackingId;
                    break;
                case ReadingState.InOccupations:
                    _occupationId = trackingId;
                    break;
            }
        }

        private void ReadToObjectEnd(ref Utf8JsonReader jsonReader, ReadingState nextState)
        {
            JsonTokenType startToken;
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    _startTokensStack.Push(JsonTokenType.StartObject);
                    break;
                case JsonTokenType.StartArray:
                    _startTokensStack.Push(JsonTokenType.StartArray);
                    break;
                case JsonTokenType.EndObject:
                    if (_startTokensStack.Count == 0)
                    {
                        _state = nextState;
                    }
                    else
                    {
                        startToken = _startTokensStack.Pop();
                        if (startToken != JsonTokenType.StartObject)
                        {
                            throw new JsonException("EndObject doesn't match start object.");
                        }
                    }
                    break;
                case JsonTokenType.EndArray:
                    startToken = _startTokensStack.Pop();
                    if (startToken != JsonTokenType.StartArray)
                    {
                        throw new JsonException("EndArray doesn't match start array.");
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReadToArrayEnd(ref Utf8JsonReader jsonReader, ReadingState nextState, int baseStackLevel = 0)
        {
            JsonTokenType startToken;
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    _startTokensStack.Push(JsonTokenType.StartObject);
                    break;
                case JsonTokenType.StartArray:
                    _startTokensStack.Push(JsonTokenType.StartArray);
                    break;
                case JsonTokenType.EndObject:
                    startToken = _startTokensStack.Pop();
                    if (startToken != JsonTokenType.StartObject)
                    {
                        throw new JsonException("EndObject doesn't match start object.");
                    }
                    break;
                case JsonTokenType.EndArray:
                    if (_startTokensStack.Count == baseStackLevel)
                    {
                        _state = nextState;
                    }
                    else
                    {
                        startToken = _startTokensStack.Pop();
                        if (startToken != JsonTokenType.StartArray)
                        {
                            throw new JsonException("EndArray doesn't match start array.");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReadTrend(ref Utf8JsonReader jsonReader)
        {
            string? name = null;
            string? unit = null;
            DateTime? startTimeUTC = null;

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                _state = ReadingState.InOccupations;
                return;
            }
            if (jsonReader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected the start of a trend.");
            }
            jsonReader.Read();

            while (TrendJsonNoValuesReadConverter.ReadBasicProperty(ref jsonReader, ref name, ref unit, ref startTimeUTC))
            {
                jsonReader.Read();
            }

            if (name != null && unit != null && startTimeUTC.HasValue)
            {
                _trendJson = new TrendJson { Name = name, Unit = unit, StartTimeUtc = startTimeUTC.Value };
                _state = ReadingState.ReadingTrend;
            }
        }

        private void ReadTrendValues(ref Utf8JsonReader jsonReader)
        {
            if (jsonReader.TokenType == JsonTokenType.PropertyName)
            {
                if (!jsonReader.ValueTextEquals("values"))
                {
                    throw new JsonException("Only expected trend values");
                }
                jsonReader.Read();
            }
            if (jsonReader.TokenType == JsonTokenType.StartArray)
            {
                jsonReader.Read();
            }
            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                jsonReader.Read();
                if (jsonReader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException("Expected end of trend object.");
                }
                _state = ReadingState.HaveTrend;
                return;
            }
            if (jsonReader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected start of trend value object.");
            }

            TimeSpan? relativeTime = null;
            double? value = null;
            while (!value.HasValue || !relativeTime.HasValue)
            {
                jsonReader.Read();
                if (jsonReader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected trend value property.");
                }
                if (jsonReader.ValueTextEquals("t"))
                {
                    jsonReader.Read();
                    relativeTime = TimeSpan.Parse(jsonReader.GetString());
                }
                else if (jsonReader.ValueTextEquals("v"))
                {
                    jsonReader.Read();
                    value = jsonReader.GetDouble();
                }
                else
                {
                    throw new JsonException($"Unexpected property {jsonReader.GetString()}");
                }
            }

            _trendJson!.Values.Add(new TrendValueJson { RelativeTime = relativeTime.Value, Value = value.Value });

            jsonReader.Read();
            if (jsonReader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException("Expected end of trend value object.");
            }
        }
    }
}
