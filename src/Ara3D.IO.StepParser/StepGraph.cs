namespace Ara3D.IO.StepParser
{
    // TODO: delete
    /*
    public class StepGraph
    {
        public StepDocument Data;
        public Dictionary<int, StepDefinition> Definitions = new();
        public MultiDictionary<int, int> Relations = new();
        public MultiDictionary<int, int> InverseRelations = new();
        public Dictionary<int, StepToken[]> Attributes = new();

        public string GetEntityName(int id)
            => Definitions[id].GetName();

        public StepGraph(StepDocument doc)
        {
            Data = doc;
            foreach (var def in doc.Definitions)
            {
                var defId = def.Id;
                Definitions.Add(defId, def);
                var attrs = def.GetAttributes(doc);
                Attributes.Add(defId, attrs.ToArray());
            }

            foreach (var kv in Attributes)
            {
                var defId = kv.Key;
                foreach (var val in kv.Value)
                {
                    if (val.IsId)
                    {
                        var valId = Data.AsId(val);
                        Relations.Add(defId, valId);
                        InverseRelations.Add(valId, defId);
                    }
                }
            }
        }
    }
    */
}
