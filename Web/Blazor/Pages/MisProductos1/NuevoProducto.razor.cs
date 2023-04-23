using Blazor.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Modelos;

namespace Blazor.Pages.MisProductos
{
    public partial class NuevoProducto
    {
        [Inject] private IProductoServicio productoServicio { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private SweetAlertService Swal { get; set; }

        Producto prod = new Producto();

        string imgUrl = string.Empty;

        private async Task SeleccionarImagen(InputFileChangeEventArgs e)
        {
            IBrowserFile imgFile = e.File;
            var buffers = new byte[imgFile.Size];
            prod.Foto = buffers;
            await imgFile.OpenReadStream().ReadAsync(buffers);
            string imageType = imgFile.ContentType;
            imgUrl = $"data:{imageType};base64,{Convert.ToBase64String(buffers)}";
        }

        protected async Task Guardar()
        {
            // whitespace = aunque el usuario ingrese espacios en blanco, el valida
            if (string.IsNullOrWhiteSpace(prod.Codigo) || string.IsNullOrWhiteSpace(prod.Descripcion))
            {
                return;
            }

            Producto prodExistente = new Producto();

            prodExistente = await productoServicio.GetPorCodigo(prod.Codigo);

            if (prodExistente != null)
            {
                if (!string.IsNullOrEmpty(prodExistente.Codigo))
                {
                    await Swal.FireAsync("Advertencia", "Ya Existe un Producto con el Mismo Código", SweetAlertIcon.Warning);
                    return;
                }
            }

            bool inserto = await productoServicio.Nuevo(prod);
            if (inserto)
            {
                await Swal.FireAsync("Atención", "Producto Guardado", SweetAlertIcon.Success);
            }
            else
            {
                await Swal.FireAsync("Error", "No se Pudo Guardar el Producto", SweetAlertIcon.Error);
            }
        }

        protected async Task Cancelar()
        {
            navigationManager.NavigateTo("/Productos");
        }
    }
}
