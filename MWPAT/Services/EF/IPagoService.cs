namespace SiteManage.Services.EF
{
    public interface IPagoService
    {
        public string ConsultarTBL_PAGO(Guid IDTransaction,string TRAMA, string Terminal);
    }
}
