using MaxiLoterias.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Models
{
    [JsonConverter(typeof(NumeroLoteriaConverter))]
    public class NumeroLoteria
    {
        string [] _RawValue;
        int Puesto { get; set; }
        int? Numero { get; set; }

        public bool HasValue() => Numero.HasValue;

        public int Name() => Puesto;
        public int Value() => Numero.Value;

        public string GetString() 
        {
            return "{" + Puesto + "º," + (Numero.HasValue ? Numero.Value.ToString() : "") + "}";
        }

        public static NumeroLoteria Parse(string content)
        {
            var arr = content.Replace("{", "").Replace("}", "").SplitBy("º,").ToArray();

            var numLot = new NumeroLoteria();

            numLot.Puesto = arr[0].Pipe(Convert.ToInt32);
            numLot.Numero = arr[1].Pipe(Convert.ToInt32);

            return numLot;
        }

        NumeroLoteria()
        {

        }

        public NumeroLoteria(string[] RawValue)
        {
            _RawValue = RawValue;
            Puesto = _RawValue[0].Replace("º", string.Empty).Pipe(Convert.ToInt32);
            Numero = _RawValue[1].Pipe(n => int.TryParse(n, out var val) ? val : (int?)null);
        }

        public NumeroLoteriaDTO ToDTO() => new NumeroLoteriaDTO(Puesto, Numero);

        public int[] AsIntArray() => Value().AsIntArray();
    }

    public class NumeroLoteriaDTO
    {
        public int Puesto { get; set; }
        public int? Numero { get; set; }

        public NumeroLoteriaDTO(int puesto, int? numero)
        {
            Puesto = puesto;
            Numero = numero;
        }
    }

    public class NumeroLoteriaConverter : JsonConverter<NumeroLoteria>
    {
        public override NumeroLoteria Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                NumeroLoteria.Parse(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer,
            NumeroLoteria numLot,
            JsonSerializerOptions options) {

            writer.WriteStartObject();            
            writer.WritePropertyName(numLot.Name() + "º");
            writer.WriteNumberValue(numLot.Value());
            writer.WriteEndObject();
        }

    }
}
