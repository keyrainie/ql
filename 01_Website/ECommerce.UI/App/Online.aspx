<%@ Page Language="C#" %>

<script lang="c#" runat="server">

    protected override void OnPreRender(EventArgs e)
    {
        try
        {
            List<ECommerce.Entity.Area> areaList = ECommerce.DataAccess.Common.CommonDA.GetAllProvince();
            if (areaList != null && areaList.Count > 0)
            {
                Response.Write("OK");
            }
            else
            {
                Response.Write("ERROR");
            }
        }
        catch (Exception)
        {

            throw;
        }

        base.OnPreRender(e);
    }

</script>
