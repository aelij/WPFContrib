using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Avalon.Internal.Utility;
using Avalon.Windows.Annotations;

namespace Avalon.Windows.Utility
{
    /// <summary>
    ///     Implements a markup extension that returns a <see cref="Type" /> based on a string attribute input.
    /// </summary>
    [MarkupExtensionReturnType(typeof (Type)), ContentProperty("TypeArguments")]
    public class TypeExtension : MarkupExtension
    {
        #region Fields

        /// <summary>
        ///     The final type returned by the <see cref="ProvideValue" /> method.
        /// </summary>
        private Type _closedType;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        public TypeExtension()
        {
            _typeArguments = new List<Type>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        public TypeExtension(string typeName)
            : this()
        {
            ArgumentValidator.NotNull(typeName, "typeName");

            _typeName = typeName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="typeArgument1">The type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1)
            : this(typeName)
        {
            ArgumentValidator.NotNull(typeArgument1, "typeArgument1");

            TypeArgument1 = typeArgument1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argumen2.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2)
            : this(typeName, typeArgument1)
        {
            ArgumentValidator.NotNull(typeArgument2, "typeArgument2");

            TypeArgument2 = typeArgument2;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argument.</param>
        /// <param name="typeArgument3">The third type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2, Type typeArgument3)
            : this(typeName, typeArgument1, typeArgument2)
        {
            ArgumentValidator.NotNull(typeArgument3, "typeArgument3");

            TypeArgument3 = typeArgument3;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeExtension" /> class.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argument.</param>
        /// <param name="typeArgument3">The third type argument.</param>
        /// <param name="typeArgument4">The fourth type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2, Type typeArgument3,
            Type typeArgument4)
            : this(typeName, typeArgument1, typeArgument2, typeArgument3)
        {
            ArgumentValidator.NotNull(typeArgument4, "typeArgument4");

            TypeArgument4 = typeArgument4;
        }

        #endregion

        #region Properties

        private string _typeName;

        /// <summary>
        ///     Gets or sets the type name.
        /// </summary>
        /// <value>The type name.</value>
        [ConstructorArgument("typeName")]
        [NotNull]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                ArgumentValidator.NotNull(value, "value");

                _typeName = value;
                _type = null;
            }
        }

        private Type _type;

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get { return _type; }
            set
            {
                ArgumentValidator.NotNull(value, "value");

                _type = value;
                _typeName = null;
            }
        }

        private readonly List<Type> _typeArguments;

        /// <summary>
        ///     Gets the type arguments.
        /// </summary>
        /// <value>The type arguments.</value>
        public IList<Type> TypeArguments
        {
            get { return _typeArguments; }
        }

        /// <summary>
        ///     Gets or sets the first type argument.
        /// </summary>
        /// <value>The first type argument.</value>
        [ConstructorArgument("typeArgument1")]
        public Type TypeArgument1
        {
            get { return GetTypeArgument(0); }
            set { SetTypeArgument(0, value); }
        }

        /// <summary>
        ///     Gets or sets the second type argument.
        /// </summary>
        /// <value>The second type argument.</value>
        [ConstructorArgument("typeArgument2")]
        public Type TypeArgument2
        {
            get { return GetTypeArgument(1); }
            set { SetTypeArgument(1, value); }
        }

        /// <summary>
        ///     Gets or sets the third type argument.
        /// </summary>
        /// <value>The third type argument.</value>
        [ConstructorArgument("typeArgument3")]
        public Type TypeArgument3
        {
            get { return GetTypeArgument(2); }
            set { SetTypeArgument(2, value); }
        }

        /// <summary>
        ///     Gets or sets the fourth type argument.
        /// </summary>
        /// <value>The fourth type argument.</value>
        [ConstructorArgument("typeArgument4")]
        public Type TypeArgument4
        {
            get { return GetTypeArgument(3); }
            set { SetTypeArgument(3, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the type argument at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Type GetTypeArgument(int index)
        {
            return index < _typeArguments.Count ? _typeArguments[index] : null;
        }

        /// <summary>
        ///     Sets the type argument at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The type.</param>
        private void SetTypeArgument(int index, Type value)
        {
            ArgumentValidator.NotNull(value, "value");

            if (index > _typeArguments.Count)
            {
                throw new ArgumentOutOfRangeException("value", SR.TypeExtension_ArgumentsWrongOrder);
            }

            if (index == _typeArguments.Count)
            {
                _typeArguments.Add(value);
            }
            else
            {
                _typeArguments[index] = value;
            }
        }

        /// <summary>
        ///     Returns the <see cref="Type" /> value as evaluated for the requested type name and type arguments.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        ///     The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_closedType == null)
            {
                if (_typeName == null && _type == null)
                {
                    throw new InvalidOperationException(SR.TypeExtension_TypeOrNameMissing);
                }

                Type type = _type;
                Type[] typeArguments = _typeArguments.TakeWhile(t => t != null).ToArray();

                if (type == null)
                {
                    // resolve using type name
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    IXamlTypeResolver typeResolver = serviceProvider != null
                        ? serviceProvider.GetService(typeof (IXamlTypeResolver)) as IXamlTypeResolver
                        : null;
                    if (typeResolver == null)
                    {
                        throw new InvalidOperationException(SR.TypeExtension_NoIXamlTypeResolver);
                    }

                    // check that the number of generic arguments match
                    string typeName = TypeName;
                    if (typeArguments.Length > 0)
                    {
                        int genericsMarkerIndex = typeName.LastIndexOf('`');
                        if (genericsMarkerIndex < 0)
                        {
                            typeName = InvariantString.Format("{0}`{1}", typeName, typeArguments.Length);
                        }
                        else
                        {
                            bool validArgumentCount = false;
                            if (genericsMarkerIndex < typeName.Length)
                            {
                                int typeArgumentCount;
                                if (int.TryParse(typeName.Substring(genericsMarkerIndex + 1), out typeArgumentCount))
                                {
                                    validArgumentCount = true;
                                }
                            }

                            if (!validArgumentCount)
                            {
                                throw new InvalidOperationException(SR.TypeExtension_InvalidTypeNameArgumentCount);
                            }
                        }
                    }

                    type = typeResolver.Resolve(typeName);
                    if (type == null)
                    {
                        throw new InvalidOperationException(SR.TypeExtension_InvalidTypeName);
                    }
                }
                else if (type.IsGenericTypeDefinition && type.GetGenericArguments().Length != typeArguments.Length)
                {
                    throw new InvalidOperationException(SR.TypeExtension_InvalidTypeArgumentCount);
                }

                // build closed type
                if (typeArguments.Length > 0 && type.IsGenericTypeDefinition)
                {
                    _closedType = type.MakeGenericType(typeArguments);
                }
                else
                {
                    _closedType = type;
                }
            }

            return _closedType;
        }

        #endregion
    }
}