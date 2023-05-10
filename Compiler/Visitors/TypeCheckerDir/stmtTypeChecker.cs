﻿using Moduino.analysis;
using Moduino.node;

namespace Compiler.Visitors.TypeCheckerDir;

public class stmtTypeChecker : DepthFirstAdapter
{
    protected SymbolTable symbolTable;
    public stmtTypeChecker(SymbolTable symbolTable)
    {
        this.symbolTable = symbolTable;
    }
    public override void OutAAssignStmt(AAssignStmt node) {
        Symbol? type = symbolTable.GetSymbol("" + node.GetId());
        Symbol? exprType = symbolTable.GetSymbol(node.GetExp());    
        
        // if type == null (id was never declared) (The reason we dont use .isInCurrentScope here is we want to iclude foward refrences
        // if type != exprType (Incompatible types)
        symbolTable.AddNode(node, type == null || type != exprType ? Symbol.notOk : Symbol.ok);
    }
    public override void OutAPlusassignStmt(APlusassignStmt node)
    {
        Symbol? type = symbolTable.GetSymbol(node.GetId());
        Symbol? exprType = symbolTable.GetSymbol(node.GetExp());
        
        symbolTable.AddNode(node,type == null ||type != exprType? Symbol.notOk: Symbol.ok);
    }
    public override void OutAMinusassignStmt(AMinusassignStmt node)
    {
        Symbol? type = symbolTable.GetSymbol(node.GetId());
        Symbol? exprType = symbolTable.GetSymbol(node.GetExp());
        
        symbolTable.AddNode(node,type == null ||type != exprType? Symbol.notOk: Symbol.ok);
    }
    public override void OutAPrefixplusStmt(APrefixplusStmt node) => UnaryoperatorToSymbolTable(node);
    public override void OutAPrefixminusStmt(APrefixminusStmt node) => UnaryoperatorToSymbolTable(node);
    public override void OutASuffixplusStmt(ASuffixplusStmt node) => UnaryoperatorToSymbolTable(node);
    public override void OutASuffixminusStmt(ASuffixminusStmt node) => UnaryoperatorToSymbolTable(node);
    public override void OutADeclStmt(ADeclStmt node)
    {
        if (!symbolTable.IsInCurrentScope(node.GetId()))
        {
            switch (node.GetType())
            {
                case AIntType:
                    symbolTable.AddId(node.GetId(), node, Symbol.Int);
                    break;
                case ADecimalType:
                    symbolTable.AddId(node.GetId(), node, Symbol.Decimal);
                    break;
                case ABoolType:
                    symbolTable.AddId(node.GetId(), node, Symbol.Bool);
                    break;
                case ACharType:
                    symbolTable.AddId(node.GetId(), node, Symbol.Char);
                    break;
                case AStringType:
                    symbolTable.AddId(node.GetId(), node, Symbol.String);
                    break;
                case AUnitType customType:
                {
                    // ----- Logic missing here----

                    break; 
                }
            }
        }
        else
        {
            symbolTable.AddId(node.GetId(), node, Symbol.notOk);
        }
    }
    public override void OutADeclassStmt(ADeclassStmt node)
    { 
        // Assignment have to be typechecked before Decl should add to symbolTable
        bool declared = symbolTable.IsInCurrentScope(node.GetId());
        if (!declared)
        {
            Symbol? exprType = symbolTable.GetSymbol(node.GetExp());
            PType unit = node.GetType();
            if (unit != null)
            {
                switch (unit)
                {
                    case AIntType a:
                        symbolTable.AddId(node.GetId(), node, exprType == Symbol.Int ? Symbol.notOk : Symbol.Int);
                        break;
                    case ADecimalType b:
                        symbolTable.AddId(node.GetId(), node, exprType == Symbol.Decimal ? Symbol.notOk : Symbol.Decimal);
                        break;
                    case ABoolType c:
                        symbolTable.AddId(node.GetId(), node, exprType == Symbol.Bool ? Symbol.notOk : Symbol.Bool);
                        break;
                    case ACharType d:
                        symbolTable.AddId(node.GetId(), node, exprType == Symbol.Char ? Symbol.notOk : Symbol.Char);
                        break;
                    case AStringType e:
                        symbolTable.AddId(node.GetId(), node, exprType == Symbol.String ? Symbol.notOk : Symbol.String);
                        break;
                    case AUnitType customType:
                    {
                        // ----- Logic missing here----

                        break; 
                    }
                }
            }
        }
        else
        {
            symbolTable.AddId(node.GetId(), node, Symbol.notOk);
        }
    }
    public override void OutAFunccallStmt(AFunccallStmt node)
    {
       // STANDALONE FUNCCAL
       // Returtype er ubetydlig
       // Om Parameterene matcher Functionens Arguments skal selvf tjekkes
       
       
    }

    public Symbol? PTypeToSymbol(PType type)
    {
        switch (type)
        {
            case AIntType:
                return Symbol.Bool;
            case ADecimalType:
                return Symbol.Decimal;
            case ABoolType:
                return Symbol.Bool;
            case ACharType:
                return Symbol.Char;
            case AStringType:
                return Symbol.String;
            case AVoidType:
                return Symbol.Func;
            default:
                return null;
        }
    }
    public bool CompareCustomUnits(Tuple<List<AUnitdeclGlobal>, List<AUnitdeclGlobal>> unit1, Tuple<List<AUnitdeclGlobal>, List<AUnitdeclGlobal>> unit2)
    { 
        List<AUnitdeclGlobal> FuncNums = unit1.Item1;
        List<AUnitdeclGlobal> ReturnNums = unit2.Item1;
        List<AUnitdeclGlobal> FuncDens = unit1.Item2;
        List<AUnitdeclGlobal> ReturnDens = unit2.Item2;
                    
        var sortedFuncNums = FuncNums.OrderBy(x => x).ToList();
        var sortedReturnNums = ReturnNums.OrderBy(x => x).ToList();
        var sortedFuncDens = FuncDens.OrderBy(x => x).ToList();
        var sortedReturnDens = ReturnDens.OrderBy(x => x).ToList();

        return sortedFuncNums.SequenceEqual(sortedReturnNums) && sortedFuncDens.SequenceEqual(sortedReturnDens);
    }

    public override void InAReturnStmt(AReturnStmt node)
    {
      //already set before
      Node parent = node.Parent();
      while (parent is not PGlobal)
      {
          parent = parent.Parent();
      }
      PExp? returnExp = node.GetExp();
      switch (parent)
      {
          case ALoopGlobal:
              // Should not have return in Loop
              symbolTable.AddNode(node, Symbol.notOk);
              break;
          case AProgGlobal:
              // Should not have return in Prog
              symbolTable.AddNode(node, Symbol.notOk);
              break;
          case ATypedGlobal aTypedFunc:
              PType typedType = aTypedFunc.GetType();
              //Symbol? symbol = symbolTable.GetSymbol(returnExp);
              Symbol? symbol = PTypeToSymbol(typedType);
              switch (symbol)
              {
                  case Symbol.Int:
                      //Int function
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.Int ? Symbol.ok : Symbol.notOk);
                      break;
                  case Symbol.Decimal:
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.Decimal ? Symbol.ok : Symbol.notOk);
                      break;
                  case Symbol.Bool:
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.Bool ? Symbol.ok : Symbol.notOk);
                      break;
                  case Symbol.Char:
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.Char ? Symbol.ok : Symbol.notOk);
                      break;
                  case Symbol.String:
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.String ? Symbol.ok : Symbol.notOk);
                      break;
                  case Symbol.Func:
                      symbolTable.AddNode(node, symbolTable.GetSymbol(returnExp) == Symbol.Func ? Symbol.ok : Symbol.notOk);
                      break;
                  default:
                      Tuple<List<AUnitdeclGlobal>, List<AUnitdeclGlobal>> funcType = symbolTable.GetUnit(typedType);
                      Tuple<List<AUnitdeclGlobal>, List<AUnitdeclGlobal>>? returnType = symbolTable.GetUnit(returnExp);
                      if (symbolTable.GetUnit(returnExp) != null)
                      {
                          if (CompareCustomUnits(funcType, returnType))
                          {
                              symbolTable.AddNode(node, Symbol.ok);
                          }
                          else
                          {
                              symbolTable.AddNode(node, Symbol.notOk);
                          }
                      }
                      else
                      {
                          symbolTable.AddNode(node, Symbol.notOk);
                      }
                      break;
              }
              break;
          case AUntypedGlobal aUntypedFunc:
              if (symbolTable.GetUnit(aUntypedFunc) != null)
              {
                  symbolTable.AddNode(node, CompareCustomUnits(symbolTable.GetUnit(aUntypedFunc), symbolTable.GetUnit(returnExp)) ? Symbol.ok : Symbol.notOk);
              } else if (symbolTable.GetReturnFromNode(aUntypedFunc) != null)
              {
                  symbolTable.AddNode(node, symbolTable.GetReturnFromNode(aUntypedFunc) == symbolTable.GetSymbol(returnExp) ? Symbol.ok : Symbol.notOk);
              }
              else
              {
                  if (symbolTable.GetUnit(returnExp) != null)
                  {
                      symbolTable.AddNodeToUnit(aUntypedFunc, symbolTable.GetUnit(returnExp));
                      symbolTable.AddNodeToUnit(parent, symbolTable.GetUnit(returnExp));
                  } else if (symbolTable.GetSymbol(returnExp) != null)
                  {
                      symbolTable.AddReturnSymbol(aUntypedFunc, symbolTable.GetSymbol(returnExp));
                      symbolTable.AddNode(parent, (Symbol)symbolTable.GetSymbol(returnExp));
                  }
              }
              break;
          default:
              symbolTable.AddNode(node, Symbol.notOk);
              break;
      }
    }

    public override void OutAIfStmt(AIfStmt node)
    {
        Symbol? CondExpr = symbolTable.GetSymbol(node.GetExp());
        symbolTable.AddNode(node,CondExpr != Symbol.Bool|| CondExpr == null ? Symbol.notOk: Symbol.ok);
    }

    public override void OutAElseifStmt(AElseifStmt node)
    {
        Symbol? CondExpr = symbolTable.GetSymbol(node.GetExp());
        symbolTable.AddNode(node,CondExpr != Symbol.Bool || CondExpr == null ? Symbol.notOk: Symbol.ok);
    }

    public override void OutAElseStmt(AElseStmt node)
    {
        Symbol? symbol = symbolTable.GetSymbol(node);
        symbolTable.AddNode(node, symbol == null ? Symbol.notOk : Symbol.ok);
    }

    public override void OutAForStmt(AForStmt node)
    {
        Symbol? cond = symbolTable.GetSymbol(node.GetCond());
        Symbol? Incr = symbolTable.GetSymbol(node.GetIncre());
        symbolTable.AddNode(node, cond != Symbol.Bool ? Symbol.notOk : Symbol.ok);
    }

    public override void OutAWhileStmt(AWhileStmt node)
    {
        Symbol? cond = symbolTable.GetSymbol(node.GetExp());
        symbolTable.AddNode(node, cond != Symbol.Bool? Symbol.notOk: Symbol.ok);
    }

    public override void OutADowhileStmt(ADowhileStmt node)
    {
        Symbol? cond = symbolTable.GetSymbol(node.GetExp());
        symbolTable.AddNode(node, cond != Symbol.Bool? Symbol.notOk: Symbol.ok);  
    }
    private void UnaryoperatorToSymbolTable(Node node)
    {
        Symbol? expr = symbolTable.GetSymbol(node);
        switch (expr)
        {
            case Symbol.Decimal:
                symbolTable.AddNode(node,Symbol.ok);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node,Symbol.ok); 
                break;
            case Symbol.Char:
                symbolTable.AddNode(node,Symbol.ok);
                break;
            default:
                symbolTable.AddNode(node,Symbol.notOk);
                break;
        }
    }
}