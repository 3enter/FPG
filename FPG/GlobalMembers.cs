using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPG
{

    //====================================================================================================
    //The Free Edition of C++ to C# Converter limits conversion output to 100 lines per file.

    //To subscribe to the Premium Edition, visit our website:
    //https://www.tangiblesoftwaresolutions.com/order/order-cplus-to-csharp.html
    //====================================================================================================

    public static class GlobalMembers
    {
        public static SortedSet<Tuple<SortedSet<string>, UInt64>> fptree_growth(FPTree fptree)
        {
            if (fptree.empty())
            {
                return { };
            }

            if (contains_single_path(fptree))
            {
                // generate all possible combinations of the items in the tree

                SortedSet<Tuple<SortedSet<string>, uint64_t>> single_path_patterns = new SortedSet<Tuple<SortedSet<string>, uint64_t>>();

                // for each node in the tree
                Debug.Assert(fptree.root.children.Count == 1);
                var curr_fpnode = fptree.root.children[0];
                while (curr_fpnode)
                {
                    string curr_fpnode_item = curr_fpnode.item;
                    uint64_t curr_fpnode_frequency = curr_fpnode.frequency;

                    // add a pattern formed only by the item of the current node
                    Tuple<SortedSet<string>, uint64_t> new_pattern = new Tuple<SortedSet<string>, uint64_t>({ curr_fpnode_item }, curr_fpnode_frequency);
                    single_path_patterns.Add(new_pattern);

                    // create a new pattern by adding the item of the current node to each pattern generated until now
                    foreach (Tuple<SortedSet<string>, uint64_t> pattern in single_path_patterns)
                    {
                        Tuple<SortedSet<string>, uint64_t> new_pattern = new Tuple<SortedSet<string>, uint64_t>(pattern.Item1, pattern.Item2);
                        new_pattern.Item1.insert(curr_fpnode_item);
                        Debug.Assert(curr_fpnode_frequency <= pattern.Item2);
                        new_pattern.Item2 = curr_fpnode_frequency;

                        single_path_patterns.Add(new_pattern);
                    }

                    // advance to the next node until the end of the tree
                    Debug.Assert(curr_fpnode.children.size() <= 1);
                    if (curr_fpnode.children.size() == 1)
                    {
                        curr_fpnode = curr_fpnode.children.front();
                    }
                    else
                    {
                        curr_fpnode = null;
                    }
                }

                return new SortedSet<Tuple<SortedSet<string>, uint64_t>>(single_path_patterns);
            }
            else
            {
                // generate conditional fptrees for each different item in the fptree, then join the results

                SortedSet<Tuple<SortedSet<string>, uint64_t>> multi_path_patterns = new SortedSet<Tuple<SortedSet<string>, uint64_t>>();

                // for each item in the fptree
                foreach (var pair in fptree.header_table)
                {
                    string curr_item = pair.first;

                    // build the conditional fptree relative to the current item

                    // start by generating the conditional pattern base
                    List<Tuple<List<string>, uint64_t>> conditional_pattern_base = new List<Tuple<List<string>, uint64_t>>();

                    // for each path in the header_table (relative to the current item)
                    var path_starting_fpnode = pair.second;
                    while (path_starting_fpnode)
                    {
                        // construct the transformed prefix path

                        // each item in th transformed prefix path has the same frequency (the frequency of path_starting_fpnode)
                        uint64_t path_starting_fpnode_frequency = path_starting_fpnode.frequency;

                        var curr_path_fpnode = path_starting_fpnode.parent.@lock();
                        // check if curr_path_fpnode is already the root of the fptree
                        if (curr_path_fpnode.parent.@lock())
                        {
                            // the path has at least one node (excluding the starting node and the root)
                            Tuple<List<string>, uint64_t> transformed_prefix_path = new Tuple<List<string>, uint64_t>({ }, path_starting_fpnode_frequency);

                            while (curr_path_fpnode.parent.@lock())
                            {
                                Debug.Assert(curr_path_fpnode.frequency >= path_starting_fpnode_frequency);
                                transformed_prefix_path.Item1.push_back(curr_path_fpnode.item);

                                // advance to the next node in the path
                                curr_path_fpnode = curr_path_fpnode.parent.@lock();
                            }

                            conditional_pattern_base.Add(transformed_prefix_path);
                        }

                        // advance to the next path
                        path_starting_fpnode = path_starting_fpnode.node_link;
                    }

                    // generate the transactions that represent the conditional pattern base
                    List<List<string>> conditional_fptree_transactions = new List<List<string>>();
                    foreach (Tuple<List<string>, uint64_t> transformed_prefix_path in conditional_pattern_base)
                    {
                        List<string> transformed_prefix_path_items = transformed_prefix_path.Item1;
                        uint64_t transformed_prefix_path_items_frequency = transformed_prefix_path.Item2;

                        List<string> transaction = new List(transformed_prefix_path_items);

                        // add the same transaction transformed_prefix_path_items_frequency times
                        for (var i = 0; i < transformed_prefix_path_items_frequency; ++i)
                        {
                            conditional_fptree_transactions.Add(transaction);
                        }
                    }

                    // build the conditional fptree relative to the current item with the transactions just generated
                    FPTree conditional_fptree = new FPTree(conditional_fptree_transactions, fptree.minimum_support_threshold);
                    // call recursively fptree_growth on the conditional fptree (empty fptree: no patterns)
                    SortedSet<Tuple<SortedSet<string>, uint64_t>> conditional_patterns = fptree_growth(conditional_fptree);

                    // construct patterns relative to the current item using both the current item and the conditional patterns
                    SortedSet<Tuple<SortedSet<string>, uint64_t>> curr_item_patterns = new SortedSet<Tuple<SortedSet<string>, uint64_t>>();

                    // the first pattern is made only by the current item
                    // compute the frequency of this pattern by summing the frequency of the nodes which have the same item (follow the node links)
                    uint64_t curr_item_frequency = 0;
                    var fpnode = pair.second;
                    while (fpnode)
                    {
                        curr_item_frequency += fpnode.frequency;
                        fpnode = fpnode.node_link;
                    }
                    // add the pattern as a result
                    Tuple<SortedSet<string>, uint64_t> pattern = new Tuple<SortedSet<string>, uint64_t>({ curr_item }, curr_item_frequency);
                    curr_item_patterns.Add(pattern);

                    // the next patterns are generated by adding the current item to each conditional pattern
                    foreach (Tuple<SortedSet<string>, uint64_t> pattern in conditional_patterns)
                    {
                        Tuple<SortedSet<string>, uint64_t> new_pattern = new Tuple<SortedSet<string>, uint64_t>(pattern.Item1, pattern.Item2);
                        new_pattern.Item1.insert(curr_item);
                        Debug.Assert(curr_item_frequency >= pattern.Item2);
                        new_pattern.Item2 = pattern.Item2;

                        curr_item_patterns.Add({ new_pattern});
                    }

                    // join the patterns generated by the current item with all the other items of the fptree
                    multi_path_patterns.insert(curr_item_patterns.cbegin(), curr_item_patterns.cend());
                }

                return new SortedSet<Tuple<SortedSet<string>, uint64_t>>(multi_path_patterns);
            }
        }

        public static bool contains_single_path(FPNode fpnode)
        {
            Debug.Assert(fpnode);
            if (fpnode.children.Count == 0)
            {
                return true;
            }
            if (fpnode.children.Count > 1)
            {
                return false;
            }
            return contains_single_path(fpnode.children[0]);
        }
        public static bool contains_single_path(FPTree fptree)
        {
            return fptree.empty() || contains_single_path(fptree.root);
        }


        public static void test_1()
        {
            string a = "A";
            string b = "B";
            string c = "C";
            string d = "D";
            string e = "E";

            List<List<string>> transactions = new List<List<string>>()
        {
            new List<string> {a, b},
            new List<string> {b, c, d},
            new List<string> {a, c, d, e},
            new List<string> {a, d, e},
            new List<string> {a, b, c},
            new List<string> {a, b, c, d},
            new List<string> {a},
            new List<string> {a, b, c},
            new List<string> {a, b, d},
            new List<string> {b, c, e}
        };

            const UInt64 minimum_support_threshold = 2;

            FPTree fptree = { transactions, minimum_support_threshold };

            SortedSet<Tuple<SortedSet<string>, uint64_t>> patterns = fptree_growth(fptree);

            Debug.Assert(patterns.Count == 19);
            Debug.Assert(patterns.count({ { a}, 8}));
            Debug.Assert(patterns.count({ { b, a}, 5}));
            Debug.Assert(patterns.count({ { b}, 7}));
            Debug.Assert(patterns.count({ { c, b}, 5}));
            Debug.Assert(patterns.count({ { c, a, b}, 3}));
            Debug.Assert(patterns.count({ { c, a}, 4}));
            Debug.Assert(patterns.count({ { c}, 6}));
            Debug.Assert(patterns.count({ { d, a}, 4}));
            Debug.Assert(patterns.count({ { d, c, a}, 2}));
            Debug.Assert(patterns.count({ { d, c}, 3}));
            Debug.Assert(patterns.count({ { d, b, a}, 2}));
            Debug.Assert(patterns.count({ { d, b, c}, 2}));
            Debug.Assert(patterns.count({ { d, b}, 3}));

            //====================================================================================================
            //End of the allowed output for the Free Edition of C++ to C# Converter.

            //To subscribe to the Premium Edition, visit our website:
            //https://www.tangiblesoftwaresolutions.com/order/order-cplus-to-csharp.html
            //====================================================================================================

        }
