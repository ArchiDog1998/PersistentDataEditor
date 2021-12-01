using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class CreateObjectItems
    {
        public SortedList<Guid, CreateObjectItem[]> InputItems { get; set; } 
        public SortedList<Guid, CreateObjectItem[]> OutputItems { get; set; }

        public CreateObjectItems()
        {
            InputItems = new SortedList<Guid, CreateObjectItem[]>();
            OutputItems = new SortedList<Guid, CreateObjectItem[]>();
        }

        public CreateObjectItems(CreateObjectItemsSave items)
        {
            InputItems = new SortedList<Guid, CreateObjectItem[]>();
            foreach (var inputPair in items.InputItems)
            {
                CreateObjectItem[] inputPairSave = new CreateObjectItem[inputPair.Value.Length];
                for (int i = 0; i < inputPair.Value.Length; i++)
                {
                    inputPairSave[i] = new CreateObjectItem(inputPair.Value[i], true);
                }
                InputItems[new Guid(inputPair.Key)] = inputPairSave;
            }

            OutputItems = new SortedList<Guid, CreateObjectItem[]>();
            foreach (var outputPair in items.OutputItems)
            {
                CreateObjectItem[] outputPairSave = new CreateObjectItem[outputPair.Value.Length];
                for (int i = 0; i < outputPair.Value.Length; i++)
                {
                    outputPairSave[i] = new CreateObjectItem(outputPair.Value[i], false);
                }
                OutputItems[new Guid(outputPair.Key)] = outputPairSave;
            }
        }
    }

    internal struct CreateObjectItemsSave
    {
        public SortedList<string, CreateObjectItemSave[]> InputItems { get; set; }
        public SortedList<string, CreateObjectItemSave[]> OutputItems { get; set; }

        public CreateObjectItemsSave(CreateObjectItems items)
        {
            InputItems = new SortedList<string, CreateObjectItemSave[]>();
            foreach (var inputPair in items.InputItems)
            {
                CreateObjectItemSave[] inputPairSave = new CreateObjectItemSave[inputPair.Value.Length];
                for (int i = 0; i < inputPair.Value.Length; i++)
                {
                    inputPairSave[i] = new CreateObjectItemSave(inputPair.Value[i]);
                }
                InputItems[inputPair.Key.ToString()] = inputPairSave;
            }

            OutputItems = new SortedList<string, CreateObjectItemSave[]>();
            foreach (var outputPair in items.OutputItems)
            {
                CreateObjectItemSave[] outputPairSave = new CreateObjectItemSave[outputPair.Value.Length];
                for (int i = 0; i < outputPair.Value.Length; i++)
                {
                    outputPairSave[i] = new CreateObjectItemSave(outputPair.Value[i]);
                }
                OutputItems[outputPair.Key.ToString()] = outputPairSave;
            }
        }
    }
}
