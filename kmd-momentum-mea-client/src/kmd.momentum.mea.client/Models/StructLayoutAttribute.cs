// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Momentum.Mea.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class StructLayoutAttribute
    {
        /// <summary>
        /// Initializes a new instance of the StructLayoutAttribute class.
        /// </summary>
        public StructLayoutAttribute()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the StructLayoutAttribute class.
        /// </summary>
        /// <param name="value">Possible values include: 'Sequential',
        /// 'Explicit', 'Auto'</param>
        /// <param name="typeId"></param>
        public StructLayoutAttribute(string value = default(string), object typeId = default(object))
        {
            Value = value;
            TypeId = typeId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Sequential', 'Explicit',
        /// 'Auto'
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "typeId")]
        public object TypeId { get; private set; }

    }
}