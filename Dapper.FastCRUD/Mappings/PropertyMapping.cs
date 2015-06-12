﻿namespace Dapper.FastCrud.Mappings
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Reeturns mapping information held for a particular property.
    /// </summary>
    public class PropertyMapping
    {
        private PropertyMappingOptions _options;
        private readonly EntityMapping _entityMapping;
        private string _databaseColumnName;
        private readonly int _order;
        private string[] _foreignKeyPropertyNames = new string[0];

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal PropertyMapping(EntityMapping entityMapping, int order, PropertyDescriptor descriptor)
        {
            _order = order;
            _entityMapping = entityMapping;
            _options = PropertyMappingOptions.None;
            _databaseColumnName = descriptor.Name;
            this.Descriptor = descriptor;
        }

        /// <summary>
        /// Gets or sets the property name that represents the foreign key to be used for fetching the referenced entity. 
        /// This can only be set on properties that have an entity type.
        /// </summary>
        public string[] ForeignKeyPropertyNames
        {
            get
            {
                return _foreignKeyPropertyNames;
            }
            set
            {
                this.ValidateState();

                this._foreignKeyPropertyNames = value??new string[0];
                if (this._foreignKeyPropertyNames.Length == 0)
                {
                    _options &= ~PropertyMappingOptions.ReferencingForeignEntity;
                }
                else
                {
                    _options |= PropertyMappingOptions.ReferencingForeignEntity;
                    this.IsExcludedFromUpdates = true;
                    this.IsExcludedFromInserts = true;
                }
            }
        }

        /// <summary>
        /// Sets the property name that represents the foreign key to be used for fetching the referenced entity. 
        /// This can only be set on properties that have an entity type.
        /// </summary>
        public PropertyMapping SetForeignKeys(params string[] propertyNames)
        {
            this.ForeignKeyPropertyNames = propertyNames;
            return this;
        }

        /// <summary>
        /// Gets a flag indicating the property is referencing a foreign entity.
        /// One can be set via <see cref="ForeignKeyPropertyNames"/>.
        /// </summary>
        public bool IsReferencingForeignEntity
        {
            get
            {
                return (_options & PropertyMappingOptions.ReferencingForeignEntity) == PropertyMappingOptions.ReferencingForeignEntity;
            }
            set
            {
                this.ValidateState();

                if (value)
                {
                    throw new InvalidOperationException($"'{nameof(IsReferencingForeignEntity)}' cannot be set. Please use '{nameof(ForeignKeyPropertyNames)}' instead.");
                }

                this.ForeignKeyPropertyNames = null;
            }
        }

        [Obsolete("Please use IsPrimaryKey or SetPrimaryKey instead.")]
        public bool IsKey
        {
            get
            {
                return this.IsPrimaryKey;
            }
            set
            {
                this.IsPrimaryKey = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating the property is mapped to a primary key.
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                return (_options & PropertyMappingOptions.KeyProperty) == PropertyMappingOptions.KeyProperty;
            }
            set
            {
                this.ValidateState();

                if (value)
                {
                    _options |= PropertyMappingOptions.KeyProperty;
                    this.IsExcludedFromUpdates = true;
                }
                else
                {
                    _options &= ~PropertyMappingOptions.KeyProperty;
                }
            }
        }

        /// <summary>
        /// Marks the property as primary key.
        /// </summary>
        public PropertyMapping SetPrimaryKey(bool isPrimaryKey = true)
        {
            this.IsPrimaryKey = isPrimaryKey;
            return this;
        }

        /// <summary>
        /// Gets or sets a flag indicating the property is mapped to a database generated field.
        /// </summary>
        public bool IsDatabaseGenerated
        {
            get
            {
                return (_options & PropertyMappingOptions.DatabaseGeneratedProperty) == PropertyMappingOptions.DatabaseGeneratedProperty;
            }
            set
            {
                this.ValidateState();

                if (value)
                {
                    _options |= PropertyMappingOptions.DatabaseGeneratedProperty;
                    this.IsExcludedFromInserts = true;
                }
                else
                {
                    _options &= ~PropertyMappingOptions.DatabaseGeneratedProperty;
                }
            }
        }

        /// <summary>
        /// Indicates that the property is mapped to a database generated field.
        /// </summary>
        public PropertyMapping SetDatabaseGenerated(bool isDatabaseGenerated = true)
        {
            this.IsDatabaseGenerated = isDatabaseGenerated;
            return this;
        }

        /// <summary>
        /// Gets or sets a flag that indicates the curent property will be excluded from updates.
        /// </summary>
        public bool IsExcludedFromInserts
        {
            get
            {
                return (_options & PropertyMappingOptions.ExcludedFromInserts) == PropertyMappingOptions.ExcludedFromInserts;
            }
            set
            {
                this.ValidateState();

                if (value)
                {
                    _options |= PropertyMappingOptions.ExcludedFromInserts;
                }
                else
                {
                    _options &= ~PropertyMappingOptions.ExcludedFromInserts;
                }
            }
        }

        /// <summary>
        /// The property will be included in insert operations.
        /// </summary>
        public PropertyMapping IncludeInInserts()
        {
            this.IsExcludedFromInserts = false;
            return this;
        }

        /// <summary>
        /// The property will be excluded from update operations.
        /// </summary>
        public PropertyMapping ExcludeFromInserts()
        {
            this.IsExcludedFromInserts = true;
            return this;
        }

        /// <summary>
        /// Gets or sets a flag that indicates the curent property will be excluded from updates.
        /// </summary>
        public bool IsExcludedFromUpdates
        {
            get
            {
                return (_options & PropertyMappingOptions.ExcludedFromUpdates) == PropertyMappingOptions.ExcludedFromUpdates;
            }
            set
            {
                this.ValidateState();

                if (value)
                {
                    _options |= PropertyMappingOptions.ExcludedFromUpdates;
                }
                else
                {
                    _options &= ~PropertyMappingOptions.ExcludedFromUpdates;
                }
            }
        }

        /// <summary>
        /// The property will be included in update operations.
        /// </summary>
        public PropertyMapping IncludeInUpdates()
        {
            this.IsExcludedFromUpdates = false;
            return this;
        }

        /// <summary>
        /// The property will be excluded from update operations.
        /// </summary>
        public PropertyMapping ExcludeFromUpdates()
        {
            this.IsExcludedFromUpdates = true;
            return this;
        }

        /// <summary>
        /// Gets the currently assigned order.
        /// </summary>
        public int Order
        {
            get
            {
                return _order;
            }
        }

        /// <summary>
        /// Gets or sets the database column name.
        /// </summary>
        public string DatabaseColumnName
        {
            get
            {
                return _databaseColumnName;
            }
            set
            {
                this.ValidateState();

                Requires.NotNullOrEmpty(value, nameof(this.DatabaseColumnName));
                _databaseColumnName = value;
                _foreignKeyPropertyNames = new string[0];
            }
        }

        /// <summary>
        /// Sets the database column name.
        /// </summary>
        public PropertyMapping SetDatabaseColumnName(string dbColumnName)
        {
            this.DatabaseColumnName = dbColumnName;
            return this;
        }

        /// <summary>
        /// Gets the property descriptor of the property attached to the entity type.
        /// </summary>
        public PropertyDescriptor Descriptor { get; private set; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName => Descriptor.Name;

        /// <summary>
        /// Gets or sets the full set of options.
        /// </summary>
        internal PropertyMappingOptions Options
        {
            get
            {
                return _options;
            }
        }

        /// <summary>
        /// Removes the current property mapping.
        /// </summary>
        public void Remove()
        {
            this.ValidateState();

            _entityMapping.RemoveProperty(this.PropertyName);
        }

        internal PropertyMapping Clone(EntityMapping newEntityMapping)
        {
            var clonedPropertyMapping = new PropertyMapping(newEntityMapping, this._order, this.Descriptor);
            clonedPropertyMapping._options = this._options;
            clonedPropertyMapping._databaseColumnName = this._databaseColumnName;
            clonedPropertyMapping._foreignKeyPropertyNames = new string[this._foreignKeyPropertyNames.Length];
            Array.Copy(this._foreignKeyPropertyNames, clonedPropertyMapping._foreignKeyPropertyNames, this._foreignKeyPropertyNames.Length);
            return clonedPropertyMapping;
        }

        protected bool Equals(PropertyMapping other)
        {
            return this._entityMapping.Equals(other._entityMapping) && this.PropertyName.Equals(other.PropertyName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((PropertyMapping)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.PropertyName.GetHashCode();
        }

        public static bool operator ==(PropertyMapping left, PropertyMapping right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyMapping left, PropertyMapping right)
        {
            return !Equals(left, right);
        }

        private void ValidateState()
        {
            if (_entityMapping.IsFrozen)
            {
                throw new InvalidOperationException("No further modifications are allowed for this entity mapping. Please clone the entity mapping instead.");
            }
        }

    }
}