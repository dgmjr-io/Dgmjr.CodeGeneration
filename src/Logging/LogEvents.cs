namespace Dgmjr.CodeGeneration.Logging.Enums;

[GenerateEnumerationRecordStruct("LogEvents", "Dgmjr.CodeGeneration.Logging")]
public enum LogEvents
{
    TransactionScopeStarted = -1,
    BeginLog = 1,
    EndLog = int.MaxValue
}
