﻿using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.DFA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.IDE.Handlers
{
    internal class IterativeAlgorithmHandler
    {
        //public event EventHandler<string> InOutDataCollected = delegate { };
        public event EventHandler<string> PrintableInOutDataCollected = delegate { };
        public event EventHandler<Exception> GenerationErrored = delegate { };

        public readonly Dictionary<IterativeAlgorithms, bool> IterativeAlgoList =
            new Dictionary<IterativeAlgorithms, bool>
            {
                {IterativeAlgorithms.ReachingDefs, false},
                {IterativeAlgorithms.ReachingExprs, false},
                {IterativeAlgorithms.ConstantPropagation, false }
            };

        private Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>> SetAlgorithmResults(ControlFlowGraph cfg)
        {
            var algorithms = new Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>>();

            if (IterativeAlgoList[IterativeAlgorithms.ReachingDefs])
            {
                var op = new ThreeAddrCode.DFA.ReachingDefinitions.Operations(cfg.Code);
                var tf = new ThreeAddrCode.DFA.ReachingDefinitions.TransferFunction(cfg.Code);
                var reachingDefs = new GenericIterativeAlgorithm<HashSet<Guid>>()
                {
                    Finish = (a, b) =>
                    {
                        var (a1, a2) = a;
                        var (b1, b2) = b;

                        return !a2.Except(b2).Any();
                    },
                    Comparer = (x, y) => !x.Except(y).Any(),
                    Fill = () => (op.Lower, op.Lower)
                };
                var output = reachingDefs.Analyze(cfg,
                    new ThreeAddrCode.DFA.ReachingDefinitions.Operations(cfg.Code),
                    new ThreeAddrCode.DFA.ReachingDefinitions.TransferFunction(cfg.Code));
                algorithms.Add(IterativeAlgorithms.ReachingDefs, output);
            }

            if (IterativeAlgoList[IterativeAlgorithms.ReachingExprs])
            {
                var reachingExprs = new ThreeAddrCode.DFA.ReachingExpressions.IterativeAlgorithm();
                var output = reachingExprs.Analyze(cfg,
                    new ThreeAddrCode.DFA.ReachingExpressions.Operations(cfg.Code),
                    new ThreeAddrCode.DFA.ReachingExpressions.TransferFunction(cfg.Code));
                algorithms.Add(IterativeAlgorithms.ReachingExprs, output);
            }

            return algorithms;
        }

        private Dictionary<IterativeAlgorithms, InOutData<ThreeAddrCode.DFA.ConstantPropogationAlt.VarInfoTable>> DictAlgorithmResults(ControlFlowGraph cfg)
        {
            var algorithms = new Dictionary<IterativeAlgorithms, InOutData<ThreeAddrCode.DFA.ConstantPropogationAlt.VarInfoTable>>();

            if (IterativeAlgoList[IterativeAlgorithms.ConstantPropagation])
            {
                var ops = new ThreeAddrCode.DFA.ConstantPropogationAlt.Operations(cfg.Code);
                var tf = new ThreeAddrCode.DFA.ConstantPropogationAlt.TransferFunction(cfg.Code);

                var constPropagation = new ThreeAddrCode.DFA.ConstantPropogationAlt.IterativeAlgorithm(ops);
                var output = constPropagation.Analyze(cfg,ops,tf);
                algorithms.Add(IterativeAlgorithms.ConstantPropagation, output);
            }

            return algorithms;
        }

        public void CollectInOutData(ControlFlowGraph cfg)
        {
            try
            {
                var setResults = SetAlgorithmResults(cfg);
                var dictResults = DictAlgorithmResults(cfg);
                string output = ResultsToString(cfg, setResults, dictResults);
                PrintableInOutDataCollected(null, output);
            }
            catch (Exception ex)
            {
                GenerationErrored(null, ex);
            }
        }

        private static string ResultsToString(ControlFlowGraph cfg,
            Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>> setResults, Dictionary<IterativeAlgorithms, InOutData<ThreeAddrCode.DFA.ConstantPropogationAlt.VarInfoTable>> dictResults)
        {
            var sb = new StringBuilder();
            foreach (var kvp in setResults)
            {
                sb.AppendLine($"Алгоритм: {kvp.Key.GetString()}\n");
                foreach (var data in kvp.Value)
                {
                    sb.AppendLine($"{data.Key} = {{");

                    sb.AppendLine("    IN = {");
                    foreach (var val in data.Value.Item1)
                        sb.AppendLine($"        {cfg.Code.LabeledCode[val]}");
                    sb.AppendLine("    }");

                    sb.AppendLine();

                    sb.AppendLine("    OUT = {");
                    foreach (var val in data.Value.Item2)
                        sb.AppendLine($"        {cfg.Code.LabeledCode[val]}");
                    sb.AppendLine("    }");

                    sb.AppendLine("}\n");
                }
            }

            foreach (var kvp in dictResults)
            {
                sb.AppendLine($"Алгоритм: {kvp.Key.GetString()}\n");
                sb.AppendLine(kvp.Value.ToString());
            }

            return sb.ToString();
        }
    }
}