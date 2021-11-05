// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Operations
{
    public interface IOperationVisitor
    {
        void VisitConstantValueOperation(ConstantValueOperation operation);

        void VisitPropertyAccessOperation(PropertyAccessOperation operation);

        void VisitArrayAccessOperation(ArrayAccessOperation operation);

        void VisitResourceIdOperation(ResourceIdOperation operation);

        void VisitResourceNameOperation(ResourceNameOperation operation);

        void VisitResourceTypeOperation(ResourceTypeOperation operation);

        void VisitResourceApiVersionOperation(ResourceApiVersionOperation operation);

        void VisitResourceReferenceOperation(ResourceReferenceOperation operation);

        void VisitSymbolicResourceReferenceOperation(SymbolicResourceReferenceOperation operation);

        void VisitResourceInfoOperation(ResourceInfoOperation operation);

        void VisitModuleNameOperation(ModuleNameOperation operation);

        void VisitModuleOutputOperation(ModuleOutputOperation operation);
    }

    public abstract class Operation
    {
        protected Operation()
        {
        }

        public abstract void Accept(IOperationVisitor visitor);
    }

    public class ConstantValueOperation : Operation
    {
        public ConstantValueOperation(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitConstantValueOperation(this);
    }

    public class PropertyAccessOperation : Operation
    {
        public PropertyAccessOperation(Operation @base, string propertyName)
        {
            Base = @base;
            PropertyName = propertyName;
        }

        public Operation Base { get; }

        public string PropertyName { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitPropertyAccessOperation(this);
    }

    public class ArrayAccessOperation : Operation
    {
        public ArrayAccessOperation(Operation @base, Operation access)
        {
            Base = @base;
            Access = access;
        }

        public Operation Base { get; }

        public Operation Access { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitArrayAccessOperation(this);
    }

    public class ResourceIdOperation : Operation
    {
        public ResourceIdOperation(ResourceMetadata metadata, SyntaxBase? indexExpression)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceIdOperation(this);
    }

    public class ResourceNameOperation : Operation
    {
        public ResourceNameOperation(ResourceMetadata metadata, SyntaxBase? indexExpression)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceNameOperation(this);
    }

    public class ResourceTypeOperation : Operation
    {
        public ResourceTypeOperation(ResourceMetadata metadata, SyntaxBase? indexExpression)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceTypeOperation(this);
    }

    public class ResourceApiVersionOperation : Operation
    {
        public ResourceApiVersionOperation(ResourceMetadata metadata, SyntaxBase? indexExpression)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceApiVersionOperation(this);
    }

    public class ResourceReferenceOperation : Operation
    {
        // reference(resourceId(...), 'apiVersion', 'full')
        // OR
        // reference(resourceId(...))
        public ResourceReferenceOperation(ResourceMetadata metadata, ResourceIdOperation resourceId, bool full)
        {
            Metadata = metadata;
            ResourceId = resourceId;
            Full = full;
        }

        public ResourceMetadata Metadata { get; }

        public ResourceIdOperation ResourceId { get; }

        public bool Full { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceReferenceOperation(this);
    }

    // reference('symbol')
    // reference('symbol[0]')
    public class SymbolicResourceReferenceOperation : Operation
    {
        public SymbolicResourceReferenceOperation(ResourceMetadata metadata, SyntaxBase? indexExpression, bool full)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
            Full = full;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public bool Full { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitSymbolicResourceReferenceOperation(this);
    }

    public class ResourceInfoOperation : Operation
    {
        public ResourceInfoOperation(ResourceMetadata metadata, SyntaxBase? indexExpression)
        {
            Metadata = metadata;
            IndexExpression = indexExpression;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceInfoOperation(this);
    }

    public class ModuleNameOperation : Operation
    {
        public ModuleNameOperation(ModuleSymbol symbol, SyntaxBase? indexExpression)
        {
            Symbol = symbol;
            IndexExpression = indexExpression;
        }

        public ModuleSymbol Symbol { get; }

        public SyntaxBase? IndexExpression { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleNameOperation(this);
    }

    public class ModuleOutputOperation : Operation
    {
        public ModuleOutputOperation(ModuleSymbol symbol, SyntaxBase? indexExpression, Operation propertyName)
        {
            Symbol = symbol;
            IndexExpression = indexExpression;
            PropertyName = propertyName;
        }

        public ModuleSymbol Symbol { get; }

        public SyntaxBase? IndexExpression { get; }

        public Operation PropertyName { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleOutputOperation(this);
    }
}
