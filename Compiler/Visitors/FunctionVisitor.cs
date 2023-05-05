using System.Collections;
using Compiler.Visitors.TypeCheckerDir;
using Moduino.analysis;
using Moduino.node;

namespace Compiler.Visitors;

// This is the second pass of the typechecker

public class FunctionVisitor : DepthFirstAdapter
{
    private SymbolTable symbolTable;
    public FunctionVisitor(SymbolTable symbolTable)
    {
        this.symbolTable = symbolTable;
    }
    // Overvej om jeg mangler at kalde base.InAxx(node);
    // Collect func declarations

    public override void OutStart(Start node) => symbolTable = symbolTable.ResetScope();
    
    public override void CaseAUntypedFunc(AUntypedFunc node)
    {
        InAUntypedFunc(node);
        OutAUntypedFunc(node);
    }
    public override void InAUntypedFunc(AUntypedFunc node)
    {
        if(symbolTable.IsInCurrentScope(node.GetId()))
        {
            symbolTable.AddId(node.GetId(), node, Symbol.notOk);
        }
        else
        {
            // Save parameters
            IList param = node.GetArg();
            if (param.Count > 0)
                symbolTable.AddFunctionParams(node.GetId(), node, param);

            symbolTable.EnterScope();
            // Tilføj parameters til local scope for funktionen
        }
    }
    public override void OutAUntypedFunc(AUntypedFunc node)
    {
        // save returntype;
        // If no return statements == Symbol.Func (void)
        // But if there is it has to be a reachable return statement in the node
        // All return statements have to evaluate to same type to be correct
        // WIP i returnStmt casen
        
        
        symbolTable = symbolTable.ExitScope();
    }
    public override void CaseATypedFunc(ATypedFunc node)
    {
        InATypedFunc(node);
        OutATypedFunc(node);
    }
    public override void InATypedFunc(ATypedFunc node)
    {
        if (symbolTable.IsInCurrentScope(node.GetId()))
        {
            symbolTable.AddId(node.GetId(), node, Symbol.notOk);
        }
        else
        {
            symbolTable = symbolTable.EnterScope();
            IList inputArgs = node.GetArg();
            if (inputArgs.Count > 0)
            {
                // Add to local scope
                foreach (AArg? argument in inputArgs)
                {
                    switch (argument.GetUnittype())
                    {
                        case ATypeUnittype type when type.GetType() is AIntType:
                            symbolTable.AddId(argument.GetId() , argument, Symbol.Int);
                            break;
                        case ATypeUnittype type when type.GetType() is ADecimalType:
                            symbolTable.AddId(argument.GetId() , argument, Symbol.Decimal);
                            break;
                        case ATypeUnittype type when type.GetType() is ABoolType:
                            symbolTable.AddId(argument.GetId() , argument, Symbol.Bool);
                            break;
                        case ATypeUnittype type when type.GetType() is ACharType:
                            symbolTable.AddId(argument.GetId() , argument, Symbol.Char);
                            break;
                        case ATypeUnittype type when type.GetType() is AStringType:
                            symbolTable.AddId(argument.GetId() , argument, Symbol.String);
                            break;
                        case AUnitUnittype customType:
                        {
                            // -----------WIP----------- //
                            
                            // Declared a custom sammensat unit (Ikke en baseunit declaration)
                            IEnumerable<ANumUnituse> numerator = customType.GetUnituse().OfType<ANumUnituse>();
                            IEnumerable<ADenUnituse> denomerator = customType.GetUnituse().OfType<ADenUnituse>();
                    
                            // Declaration validering for sammensat unit her
                            // Check if Numerators or denomarots contains units that does not exist

                            symbolTable.AddNumerators(argument.GetId(), argument, numerator);
                            symbolTable.AddDenomerators(argument.GetId(), argument, denomerator);
                            break; 
                        }
                    }
                }
                // Save parameters in table
                // Change functionidToParams Dictionary to <string, List<PUnittype>> 
                symbolTable.AddFunctionParams(node.GetId(), node, inputArgs);
            }
        }
    }
    public override void OutATypedFunc(ATypedFunc node)
    {
        symbolTable = symbolTable.ExitScope();
        // Save returntype
        // But if not void it has to have a reachable return statement in the node
        // All return statements have to evaluate to same type to be correct
        
        PUnittype returnSymbol = node.GetUnittype();
        
        symbolTable.AddId(node.GetId(), node, Symbol.String);
        
    }
    public override void InALoopFunc(ALoopFunc node) => symbolTable = symbolTable.EnterScope();
    public override void OutALoopFunc(ALoopFunc node) => symbolTable = symbolTable.ExitScope();
    public override void InAProgFunc(AProgFunc node) => symbolTable = symbolTable.EnterScope();
    public override void OutAProgFunc(AProgFunc node) => symbolTable = symbolTable.ExitScope();
    
}