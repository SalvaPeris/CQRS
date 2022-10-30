using System;
namespace ApplicationCore.Common.Domain
{
	public class BaseEntidad
	{
		public DateTime? CreadoEn { get; set; }

		public string? CreadoPor { get; set; }

        public DateTime? ModificadoEn { get; set; }

        public string? ModificadoPor { get; set; }

    }
}

