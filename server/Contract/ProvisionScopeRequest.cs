using System.Collections.Generic;

namespace Server.Contract
{
    public class ProvisionScopeRequest
    {
        public string ScopeName { get; set; }

        public IDictionary<string, IEnumerable<string>> ColumnsPerTableStructure { get; set; }

        public SetupFilterDto[] SetupFilters { get; set; }
    }

    public class SetupFilterDto
    {
        public SetupFilterDto(string tableName)
        {
            TableName = tableName;
            CustomWheres = new List<string>();
        }

        public string TableName { get; set; }

        public List<string> CustomWheres { get; set; }

        public void AddCustomWhere(string customWhere)
        {
            CustomWheres.Add(customWhere);
        }
    }
}
