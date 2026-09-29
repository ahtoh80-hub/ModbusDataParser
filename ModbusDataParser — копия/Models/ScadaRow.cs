namespace ModbusDataParser.Models
{
    public class ScadaRow
    {
        public string Number { get; set; } = "";
        public string Status { get; set; } = "-";
        public string Mode { get; set; } = "Mode";
        public string Brand { get; set; } = "";
        public string ObjectType { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ObjSign { get; set; } = "";
        public string ObjNumber { get; set; } = "0";
        public string PlcVarName { get; set; } = "";
        public string ArhPer { get; set; } = "0";
        public string Kks { get; set; } = "";
        public string ObjDParam { get; set; } = "";
        public string SrezControl { get; set; } = "1";
        public string UserGroup { get; set; } = "[ВСЕ]";
        public string EvGroup { get; set; } = "[ВСЕ]";
        public string PlcName { get; set; } = "";
        public string PlcAdress { get; set; } = "";
        public string PlcGr { get; set; } = "";
    }
}
