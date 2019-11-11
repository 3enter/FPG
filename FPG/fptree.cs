using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class FPNode
    {
        public readonly string item;
        public UInt64 frequency = new UInt64();
        public FPNode node_link;
        public std::weak_ptr<FPNode> parent = new std::weak_ptr<FPNode>();
        public List<FPNode> children = new List<FPNode>();

        public FPNode(string item, FPNode parent)
        {
            this.item = item;
            this.frequency = 1;
            this.node_link = null;
            this.parent = parent;
            this.children = new List<FPNode>();
        }
    }

    public class FPTree
    {
        public FPNode root;
        public SortedDictionary<string, FPNode> header_table = new SortedDictionary<string, FPNode>();
        public UInt64 minimum_support_threshold = new UInt64();

        public class frequency_comparator
        {
            //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
            //ORIGINAL LINE: bool operator ()(const System.Tuple<string, uint64_t> &lhs, const System.Tuple<string, uint64_t> &rhs) const
            public static bool functorMethod(Tuple<string, UInt64> lhs, Tuple<string, UInt64> rhs)
            {
                return std::tie(lhs.Item2, lhs.Item1) > std::tie(rhs.Item2, rhs.Item1);
            }
        }
        public FPTree(List<List<string>> transactions, UInt64 minimum_support_threshold)
        {
            this.root = new FPNode(Item({ }), null);
            this.header_table = new SortedDictionary<string, FPNode>();
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
            //ORIGINAL LINE: this.minimum_support_threshold = minimum_support_threshold;
            this.minimum_support_threshold.CopyFrom(minimum_support_threshold);
            // scan the transactions counting the frequency of each item
            SortedDictionary<string, UInt64> frequency_by_item = new SortedDictionary<string, UInt64>();
            foreach (List<string> transaction in transactions)
            {
                foreach (string item in transaction)
                {
                    ++frequency_by_item[item];
                }
            }

            // keep only items which have a frequency greater or equal than the minimum support threshold
            for (var it = frequency_by_item.cbegin(); it != frequency_by_item.cend();)
            {
                UInt64 item_frequency = it.second;
                if (item_frequency < minimum_support_threshold)
                {
                    frequency_by_item.Remove(it++);
                }
                else
                {
                    ++it;
                }
            }

            // order items by decreasing frequency
            SortedSet<Tuple<string, UInt64>, frequency_comparator> items_ordered_by_frequency = new SortedSet<Tuple<string, UInt64>, frequency_comparator>(frequency_by_item.cbegin(), frequency_by_item.cend());

            // start tree construction

            // scan the transactions again
            foreach (List<string> transaction in transactions)
            {
                var curr_fpnode = root;

                // select and sort the frequent items in transaction according to the order of items_ordered_by_frequency
                foreach (var pair in items_ordered_by_frequency)
                {
                    string item = pair.first;

                    // check if item is contained in the current transaction
                    if (std::find(transaction.cbegin(), transaction.cend(), item) != transaction.cend())
                    {
                        // insert item in the tree

                        // check if curr_fpnode has a child curr_fpnode_child such that curr_fpnode_child.item = item
                        //C++ TO C# CONVERTER TODO TASK: Only lambda expressions having all locals passed by reference can be converted to C#:
                        //ORIGINAL LINE: const auto it = std::find_if(curr_fpnode->children.cbegin(), curr_fpnode->children.cend(), [item](const FPNode*& fpnode)
                        var it = std::find_if(curr_fpnode.children.cbegin(), curr_fpnode.children.cend(), (FPNode fpnode) =>
                        {
                            return fpnode.item == item;
                        });
                        if (it == curr_fpnode.children.cend())
                        {
                            // the child doesn't exist, create a new node
                            var curr_fpnode_new_child = new FPNode(item, curr_fpnode);

                            // add the new node to the tree
                            curr_fpnode.children.Add(curr_fpnode_new_child);

                            // update the node-link structure
                            if (header_table.count(curr_fpnode_new_child.item))
                            {
                                var prev_fpnode = header_table[curr_fpnode_new_child.item];
                                while (prev_fpnode.node_link != null)
                                {
                                    //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
                                    //ORIGINAL LINE: prev_fpnode = prev_fpnode->node_link;
                                    prev_fpnode.CopyFrom(prev_fpnode.node_link);
                                }
                                prev_fpnode.node_link = curr_fpnode_new_child;
                            }
                            else
                            {
                                header_table[curr_fpnode_new_child.item] = curr_fpnode_new_child;
                            }

                            // advance to the next node of the current transaction
                            //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
                            //ORIGINAL LINE: curr_fpnode = curr_fpnode_new_child;
                            curr_fpnode.CopyFrom(curr_fpnode_new_child);
                        }
                        else
                        {
                            // the child exist, increment its frequency
                            var curr_fpnode_child = *it;
                            ++curr_fpnode_child.frequency;

                            // advance to the next node of the current transaction
                            curr_fpnode = curr_fpnode_child;
                        }
                    }
                }
            }
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: bool empty() const
        public bool empty()
        {
            Debug.Assert(root);
            return root.children.Count == 0;
        }
    }
}
