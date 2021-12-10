using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class CreateObjectItems
    {
        public SortedList<Guid, CreateObjectItem[]> InputItems { get; set; } = new SortedList<Guid, CreateObjectItem[]>();
        public SortedList<Guid, CreateObjectItem[]> OutputItems { get; set; } = new SortedList<Guid, CreateObjectItem[]>();
        public CreateObjectItem[] ListItems { get; set; } = new CreateObjectItem[0];
        public CreateObjectItem[] TreeItems { get; set; } = new CreateObjectItem[0];
        public CreateObjectItems()
        {
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

            if (items.ListItems != null)
            {
                CreateObjectItem[] list = new CreateObjectItem[items.ListItems.Length];
                for (int i = 0; i < items.ListItems.Length; i++)
                {
                    list[i] = new CreateObjectItem(items.ListItems[i], false);
                }
                ListItems = list;
            }

            if (items.TreeItems != null)
            {
                CreateObjectItem[] tree = new CreateObjectItem[items.TreeItems.Length];
                for (int i = 0; i < items.TreeItems.Length; i++)
                {
                    tree[i] = new CreateObjectItem(items.TreeItems[i], false);
                }
                TreeItems = tree;
            }
        }
    }

    internal struct CreateObjectItemsSave
    {
        public SortedList<string, CreateObjectItemSave[]> InputItems { get; set; }
        public SortedList<string, CreateObjectItemSave[]> OutputItems { get; set; }
        public CreateObjectItemSave[] ListItems { get; set; }
        public CreateObjectItemSave[] TreeItems { get; set; }

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

            CreateObjectItemSave[] list = new CreateObjectItemSave[items.ListItems.Length];
            for (int i = 0; i < items.ListItems.Length; i++)
            {
                list[i] = new CreateObjectItemSave(items.ListItems[i]);
            }
            ListItems = list;

            CreateObjectItemSave[] tree = new CreateObjectItemSave[items.TreeItems.Length];
            for (int i = 0; i < items.TreeItems.Length; i++)
            {
                tree[i] = new CreateObjectItemSave(items.TreeItems[i]);
            }
            TreeItems = tree;
        }
    }
}
