namespace ConvulationCode
{
    class Convulation
    {
        private static Dictionary<string, Dictionary<char, (string code, string state)>> FSMCodes = new()
        {
            {
                "00", new Dictionary<char, (string code, string state)>
                {
                    { '1', ("11", "10") },
                    { '0', ("00", "00") }
                }
            },
            {
                "10", new Dictionary<char, (string code, string state)>
                {
                    { '1', ("01", "11") },
                    { '0', ("10", "01") }
                }
            },
            {
                "11", new Dictionary<char, (string code, string state)>
                {
                    { '1', ("10", "11") },
                    { '0', ("01", "01") }
                }
            },
            {
                "01", new Dictionary<char, (string code, string state)>
                {
                    { '1', ("00", "10") },
                    { '0', ("11", "00") }
                }
            }
        };

        public static string Encode(string input)
        {
            string current_state = "00";
            string result_code = "";

            foreach (char bit in input)
            {
                result_code += FSMCodes[current_state][bit].code;
                current_state = FSMCodes[current_state][bit].state;
            }

            return result_code;
        }

        static int HammingDistance(string input1, string input2)
        {
            return input1.Zip(input2, (char1, char2) => char1 != char2 ? 1 : 0).Sum();
        }

        public static string Decode(string input)
        {
            List<string> input_list = [];

            for (int i = 0; i < input.Length; i += 2)
            {
                input_list.Add(input.Substring(i, 2));
            }

            List<Dictionary<string, object>> edges = [];
            int length = input_list.Count;

            void Find(string state, int i)
            {
                if (i >= length)
                    return;

                foreach (char index in new char[] { '0', '1' })
                {
                    string nextCode = FSMCodes[state][index].code;
                    string nextState = FSMCodes[state][index].state;
                    int metric = HammingDistance(input_list[i], nextCode);
                    edges.Add(new Dictionary<string, object>
                    {
                        { "from", state },
                        { "to", nextState },
                        { "index", i },
                        { "code", nextCode },
                        { "input", input_list[i] },
                        { "value", index },
                        { "metric", metric }
                    });
                    Find(nextState, i + 1);
                }
            }

            Find("00", 0);

            List<List<Dictionary<string, object>>> paths = new List<List<Dictionary<string, object>>>();
            List<Dictionary<string, object>> path = new List<Dictionary<string, object>>();

            for (int i = 0; i < edges.Count; i++)
            {
                path.Add(edges[i]);
                if (Convert.ToInt32(edges[i]["index"]) == length - 1)
                {
                    paths.Add(new List<Dictionary<string, object>>(path));
                    if (edges.Count > i + 1)
                    {
                        for (int j = 0; j < length - Convert.ToInt32(edges[i + 1]["index"]); j++)
                        {
                            path.RemoveAt(path.Count - 1);
                        }
                    }
                }
            }
            foreach (var p in paths)
            {
                int totalMetric = p.Sum(edge => Convert.ToInt32(edge["metric"]));
                p.Add(new Dictionary<string, object> { { "totalMetric", totalMetric } });
            }

            paths.Sort((x, y) => Convert.ToInt32(x.Last()["totalMetric"]).CompareTo(Convert.ToInt32(y.Last()["totalMetric"])));

            return string.Join("", paths[0].Where(item => item.ContainsKey("value")).Select(item => item["value"]));
        }
    }
}
