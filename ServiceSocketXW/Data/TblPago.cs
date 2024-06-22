using System;
using System.Collections.Generic;

namespace ServiceSocketXW.Data;

public partial class TblPago
{
    public decimal Id { get; set; }

    public int TipoPago { get; set; }

    public string Agencia { get; set; } = null!;

    public string? Terminal { get; set; }

    public string? Msgid { get; set; }

    public decimal Valor { get; set; }

    public decimal? ValorChq { get; set; }

    public DateTime? FechaPago { get; set; }

    public DateTime FechaIngreso { get; set; }

    public string? Pagado { get; set; }

    public string? Documento { get; set; }

    public string? Referencia2 { get; set; }

    public string? Referencia3 { get; set; }

    public string? CuentaDestino { get; set; }

    public string? CuentaOrigen { get; set; }

    public string? Autorizacion { get; set; }

    public decimal? Comision { get; set; }

    public string? Enviado { get; set; }

    public string? Referencia4 { get; set; }

    public string Autorizaciontercero { get; set; } = null!;

    public decimal Refval1 { get; set; }

    public decimal Refval2 { get; set; }

    public decimal Refval3 { get; set; }

    public decimal Refval4 { get; set; }

    public decimal Efectivo { get; set; }

    public decimal Propios { get; set; }

    public decimal Ajenos { get; set; }

    public decimal Girosext { get; set; }

    public string ScoCajero { get; set; } = null!;
}
