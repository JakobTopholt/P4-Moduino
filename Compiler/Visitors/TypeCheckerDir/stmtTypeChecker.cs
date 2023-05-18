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
        Symbol? idToType = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        PExp exp = node.GetExp();
        switch (idToType)
        {
            case Symbol.NotOk:
                symbolTable.AddNode(node, Symbol.NotOk);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Int ? Symbol.Int : Symbol.NotOk);
                break;
            case Symbol.Decimal:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Decimal ? Symbol.Decimal : Symbol.NotOk);
                break;
            case Symbol.Bool:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Bool ? Symbol.Bool : Symbol.NotOk);
                break;
            case Symbol.Char:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Char ? Symbol.Char : Symbol.NotOk);
                break;
            case Symbol.String:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) is Symbol.String or Symbol.Char ? Symbol.String : Symbol.NotOk);
                break;
            default:
                if (symbolTable.GetNodeFromId(node.GetId().ToString().Trim(), out var typeUn) &&
                    symbolTable.GetUnit(typeUn, out var typeUnit) &&
                    symbolTable.GetUnit(exp, out var expUnit))
                {
                    symbolTable.AddNode(node, symbolTable.CompareCustomUnits(typeUnit, expUnit) ? Symbol.Ok : Symbol.NotOk);
                }
                else
                    symbolTable.AddNode(node, Symbol.NotOk);
                break;
        }
    }
    public override void OutAPlusassignStmt(APlusassignStmt node)
    {
        // OVervej om man kan sige += med string, char og bool typer
        Symbol? type = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        PExp exp = node.GetExp();
        switch (type)
        {
            case Symbol.Int:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Int ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Decimal:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Decimal ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Bool:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Bool ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Char:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Char ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.String:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) is Symbol.String or Symbol.Char ? Symbol.String : Symbol.NotOk);
                break;
            default:
                symbolTable.GetNodeFromId(node.GetId().ToString(), out var b);
                symbolTable.GetUnit(b, out var typeUnit);
                symbolTable.GetUnit(exp, out var expUnit);
                symbolTable.AddNode(node, symbolTable.CompareCustomUnits(typeUnit, expUnit) ? Symbol.Ok : Symbol.NotOk);
                break;
        }
    }
    public override void OutAMinusassignStmt(AMinusassignStmt node)
    {
        Symbol? type = symbolTable.GetSymbol("" + node.GetId().ToString().Trim());
        PExp exp = node.GetExp();
        switch (type)
        {
            case Symbol.Int:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Int ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Decimal:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Decimal ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Bool:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Bool ? Symbol.Ok : Symbol.NotOk);
                break;
            case Symbol.Char:
                symbolTable.AddNode(node, symbolTable.GetSymbol(exp) == Symbol.Char ? Symbol.Ok : Symbol.NotOk);
                break;
            default:
                symbolTable.GetNodeFromId(node.GetId().ToString(), out var b);
                symbolTable.GetUnit(b, out var typeUnit);
                symbolTable.GetUnit(exp, out var expUnit);
                symbolTable.AddNode(node, symbolTable.CompareCustomUnits(typeUnit, expUnit) ? Symbol.Ok : Symbol.NotOk);
                break;
        }
    }
    public override void OutAPrefixplusStmt(APrefixplusStmt node)
    {
        Symbol? expr = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        switch (expr)
        {
            case Symbol.Decimal:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node,Symbol.Ok); 
                break;
            case Symbol.Char:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            default:
                symbolTable.AddNode(node,Symbol.NotOk);
                break;
        }
    }

    public override void OutAPrefixminusStmt(APrefixminusStmt node)
    {
        Symbol? expr = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        switch (expr)
        {
            case Symbol.Decimal:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node,Symbol.Ok); 
                break;
            case Symbol.Char:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            default:
                symbolTable.AddNode(node,Symbol.NotOk);
                break;
        }
    }

    public override void OutASuffixplusStmt(ASuffixplusStmt node)
    {
        Symbol? expr = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        switch (expr)
        {
            case Symbol.Decimal:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node,Symbol.Ok); 
                break;
            case Symbol.Char:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            default:
                symbolTable.AddNode(node,Symbol.NotOk);
                break;
        }
    }

    public override void OutASuffixminusStmt(ASuffixminusStmt node)
    {
        Symbol? expr = symbolTable.GetSymbol(node.GetId().ToString().Trim());
        switch (expr)
        {
            case Symbol.Decimal:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            case Symbol.Int:
                symbolTable.AddNode(node,Symbol.Ok); 
                break;
            case Symbol.Char:
                symbolTable.AddNode(node,Symbol.Ok);
                break;
            default:
                symbolTable.AddNode(node,Symbol.NotOk);
                break;
        }
    }

    public override void OutADeclStmt(ADeclStmt node)
    {
        if (!symbolTable.IsInCurrentScope(node.GetId()))
        {
            switch (node.GetType())
            {
                case AIntType:
                    symbolTable.AddId(node.GetId().ToString(), node, Symbol.Int);
                    break;
                case ADecimalType:
                    symbolTable.AddId(node.GetId().ToString(), node, Symbol.Decimal);
                    break;
                case ABoolType:
                    symbolTable.AddId(node.GetId().ToString(), node, Symbol.Bool);
                    break;
                case ACharType:
                    symbolTable.AddId(node.GetId().ToString(), node, Symbol.Char);
                    break;
                case AStringType:
                    symbolTable.AddId(node.GetId().ToString(), node, Symbol.String);
                    break;
                case AUnitType customType:
                    symbolTable.GetUnit(customType, out var unit);
                    symbolTable.AddIdToNode(node.GetId().ToString(), node);
                    symbolTable.AddNodeToUnit(node, unit);
                    symbolTable.AddNode(node, Symbol.Ok);
                    break; 
                default:
                    symbolTable.AddNode(node, Symbol.NotOk);
                    break;
            }
        }
        else
        {
            symbolTable.AddNode(node, Symbol.NotOk);
        }
    }
    public override void OutADeclassStmt(ADeclassStmt node)
    { 
        // Assignment have to be typechecked before Decl should add to symbolTable
        if (!symbolTable.IsInCurrentScope(node.GetId()))
        {
            Symbol? exprType = symbolTable.GetSymbol(node.GetExp());
            if (node.GetExp() is AIdExp id)
            {
                exprType = symbolTable.GetSymbol(id.GetId().ToString().Trim());
            }
            PType unit = node.GetType();
            switch (unit)
                {
                    case AIntType a:
                        symbolTable.AddId(node.GetId().ToString(), node, exprType == Symbol.Int ? Symbol.Int : Symbol.NotOk);
                        break;
                    case ADecimalType b:
                        symbolTable.AddId(node.GetId().ToString(), node, exprType == Symbol.Decimal ? Symbol.Decimal : Symbol.NotOk);
                        break;
                    case ABoolType c:
                        symbolTable.AddId(node.GetId().ToString(), node, exprType == Symbol.Bool ? Symbol.Bool : Symbol.NotOk);
                        break;
                    case ACharType d:
                        symbolTable.AddId(node.GetId().ToString(), node, exprType == Symbol.Char ? Symbol.Char : Symbol.NotOk);
                        break;
                    case AStringType e:
                        symbolTable.AddId(node.GetId().ToString(), node, exprType is Symbol.String or Symbol.Char ? Symbol.String : Symbol.NotOk);
                        break;
                    case AUnitType customType:
                        //throw new Exception("bruh");
                        if (symbolTable.GetUnit(customType, out var unitType) && 
                            symbolTable.GetUnit(node.GetExp(), out var expType))
                        {
                            if (symbolTable.CompareCustomUnits(unitType, expType))
                            {
                                symbolTable.AddIdToNode(node.GetId().ToString(), node);
                                symbolTable.AddNodeToUnit(node, unitType);
                                symbolTable.AddNode(node, Symbol.Ok); 
                            }
                            else
                                symbolTable.AddNode(node, Symbol.NotOk);
                        }
                        else
                            symbolTable.AddNode(node, Symbol.NotOk);
                        break; 
                    default:
                        symbolTable.AddNode(node, Symbol.NotOk);
                        break;
                }
        }
        else
        {
            symbolTable.AddNode(node, Symbol.NotOk);
        }
    }
    public override void OutAFunccallStmt(AFunccallStmt node)
    {
       // STANDALONE FUNCCAL
       // Returtype er ubetydlig
       symbolTable.GetNodeFromId(node.GetId().ToString().Trim(), out Node? func);
       if (func != null)
       {
           List<PType>? args = symbolTable.GetFunctionArgs(func);
           List<PExp> parameters = node.GetParams().OfType<PExp>().ToList();
           bool matches = true;
           for(int i = 0; i < args.Count(); i++)
           {
               Node? parameter = parameters[i];
               if (parameters[i] is AIdExp id)
               {
                   symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out parameter);
               }
                       
               switch (args[i])
               {
                   case AIntType:
                       if (symbolTable.GetSymbol(parameter) != Symbol.Int)
                           matches = false;
                       break;
                   case ADecimalType:
                       if (symbolTable.GetSymbol(parameter) != Symbol.Decimal)
                           matches = false;
                       break;
                   case ABoolType:
                       if (symbolTable.GetSymbol(parameter) != Symbol.Bool)
                           matches = false;
                       break;
                   case ACharType:
                       if (symbolTable.GetSymbol(parameter) != Symbol.Char)
                           matches = false;
                       break;
                   case AStringType:
                       if (symbolTable.GetSymbol(parameter) != Symbol.String)
                           matches = false;
                       break;
                   case AUnitType argType:
                   {
                       symbolTable.GetUnit(argType, out var argUnit);
                       symbolTable.GetUnit(parameter, out var paramUnit);
                       if (!symbolTable.CompareCustomUnits(argUnit, paramUnit))
                       {
                           matches = false;
                           symbolTable.AddNodeToUnit(node, argUnit);
                       }
                       break; 
                   }
                   default:
                       matches = false;
                       break;
               }
           }
           symbolTable.AddNode(node, matches ? Symbol.Ok : Symbol.NotOk);
       }
       else
       {
           symbolTable.AddNode(node, Symbol.NotOk);
       }
    }

    public override void InAReturnStmt(AReturnStmt node)
    {
      //already set before
      Node parent = node.Parent();
      while (parent is not PGlobal)
      {
          parent = parent.Parent();
      }
      Node? returnExp = node.GetExp();
      if (returnExp is AIdExp id)
      {
          symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out returnExp);
      }
      Symbol? returnSymbol = symbolTable.GetSymbol(returnExp);
      switch (parent)
      {
          case ALoopGlobal:
              // Should not have return in Loop
              symbolTable.AddNode(node, Symbol.NotOk);
              break;
          case AProgGlobal:
              // Should not have return in Prog
              symbolTable.AddNode(node, Symbol.NotOk);
              break;
          case ATypedGlobal aTypedFunc:
              PType typedType = aTypedFunc.GetType();
              switch (typedType)
              {
                  case AIntType:
                      symbolTable.AddNode(node, returnSymbol == Symbol.Int ? Symbol.Ok : Symbol.NotOk);
                      break;
                  case ADecimalType:
                      symbolTable.AddNode(node, returnSymbol == Symbol.Decimal ? Symbol.Ok : Symbol.NotOk);
                      break;
                  case ABoolType:
                      symbolTable.AddNode(node, returnSymbol == Symbol.Bool ? Symbol.Ok : Symbol.NotOk);
                      break;
                  case ACharType:
                      symbolTable.AddNode(node, returnSymbol == Symbol.Char ? Symbol.Ok : Symbol.NotOk);
                      break;
                  case AStringType:
                      symbolTable.AddNode(node, returnSymbol == Symbol.String ? Symbol.Ok : Symbol.NotOk);
                      break;
                  case AVoidType:
                      symbolTable.AddNode(node, Symbol.NotOk);
                      break;
                  case AUnitType:
                      symbolTable.GetUnit(typedType, out (List<AUnitdeclGlobal>, List<AUnitdeclGlobal>) funcType);
                      if (symbolTable.GetUnit(returnExp, out (List<AUnitdeclGlobal>, List<AUnitdeclGlobal>) returnType))
                          symbolTable.AddNode(node, symbolTable.CompareCustomUnits(funcType, returnType) ? Symbol.Ok : Symbol.NotOk);
                      else
                          symbolTable.AddNode(node, Symbol.NotOk);
                      break;
                  default:
                      symbolTable.AddNode(node, Symbol.NotOk);
                      break;
              }
              break;
          case AUntypedGlobal aUntypedFunc:
              if (symbolTable.GetUnit(aUntypedFunc, out var func))
              {
                  symbolTable.GetUnit(returnExp, out var expUnit);
                  symbolTable.AddNode(node, symbolTable.CompareCustomUnits(func, expUnit) ? Symbol.Ok : Symbol.NotOk);
              } else if (symbolTable.GetReturnFromNode(aUntypedFunc) != null)
              {
                  symbolTable.AddNode(node, symbolTable.GetReturnFromNode(aUntypedFunc) == symbolTable.GetSymbol(returnExp) ? Symbol.Ok : Symbol.NotOk);
              }
              else
              {
                  if (symbolTable.GetUnit(returnExp, out var expUnit))
                  {
                      symbolTable.AddNodeToUnit(aUntypedFunc, expUnit);
                      symbolTable.AddNode(node, Symbol.Ok);
                  } else if (symbolTable.GetSymbol(returnExp) != null)
                  {
                      symbolTable.AddReturnSymbol(aUntypedFunc, symbolTable.GetSymbol(returnExp));
                      symbolTable.AddNode(node, (Symbol)symbolTable.GetSymbol(returnExp));
                  }
              }
              break;
          default:
              symbolTable.AddNode(node, Symbol.NotOk);
              break;
      }
    }
    public override void OutAIfStmt(AIfStmt node)
    {
        Symbol? condExpr = symbolTable.GetSymbol(node.GetExp());
        if (node.GetExp() is AIdExp id)
        {
            condExpr = symbolTable.GetSymbol(id.ToString().Trim());
        }
        symbolTable.AddNode(node, condExpr == Symbol.Bool ? Symbol.Bool: Symbol.NotOk);
    }
    public override void OutAElseifStmt(AElseifStmt node)
    {
        Symbol? condExpr = symbolTable.GetSymbol(node.GetExp());
        if (node.GetExp() is AIdExp id)
        {
            condExpr = symbolTable.GetSymbol(id.ToString().Trim());
        }
        symbolTable.AddNode(node, condExpr == Symbol.Bool ? Symbol.Bool: Symbol.NotOk);
    }
    public override void OutAElseStmt(AElseStmt node)
    {
        // Check om der er en if eller else if inden
        symbolTable.AddNode(node, Symbol.Ok);
    }
    public override void OutAForStmt(AForStmt node)
    {
        Symbol? condExpr = symbolTable.GetSymbol(node.GetCond());
        symbolTable.AddNode(node, condExpr == Symbol.Bool ? Symbol.Bool: Symbol.NotOk);
    }
    public override void OutAWhileStmt(AWhileStmt node)
    {
        Node? exp = node.GetExp();
        if (exp is AIdExp id)
        {
            symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out exp);
        }
        Symbol? condExpr = symbolTable.GetSymbol(exp);
        symbolTable.AddNode(node, condExpr == Symbol.Bool ? Symbol.Bool: Symbol.NotOk);
    }
    public override void OutADowhileStmt(ADowhileStmt node)
    {
        Node? exp = node.GetExp();
        if (exp is AIdExp id)
        {
            symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out exp);
        }
        Symbol? condExpr = symbolTable.GetSymbol(exp);
        symbolTable.AddNode(node, condExpr == Symbol.Bool ? Symbol.Bool: Symbol.NotOk);
    }
    public override void OutADelayStmt(ADelayStmt node)
    {
        Node? exp = node.GetExp();
        if (exp is AIdExp id)
        {
            symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out exp);
        }
        // Missing logic 
        
    }
    public override void OutASetpinStmt(ASetpinStmt node)
    {
        Node? exp = node.GetExp();
        if (exp is AIdExp id)
        {
            symbolTable.GetNodeFromId(id.GetId().ToString().Trim(), out exp);
        }
        // missing logic
        
        
    }
}