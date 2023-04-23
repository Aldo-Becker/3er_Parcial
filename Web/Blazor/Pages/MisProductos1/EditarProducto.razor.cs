using Blazor.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Modelos;

namespace Blazor.Pages.MisProductos1
{
    public partial class EditarProducto
    {
        [Inject] private IProductoServicio productoServicio { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private SweetAlertService Swal { get; set; }

        Producto prod = new Producto();

        [Parameter] public string Codigo { get; set; }

        string imgUrl = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(Codigo))
            {
                prod = await productoServicio.GetPorCodigo(Codigo);
            }
        }

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
            if (string.IsNullOrWhiteSpace(prod.Codigo) || string.IsNullOrWhiteSpace(prod.Descripcion))
            {
                return;
            }

            bool edito = await productoServicio.Actualizar(prod);
            if (edito)
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

        protected async Task Eliminar()
        {
            bool elimino = false;

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "¿Seguro Que Desea Eliminar el Producto?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                ConfirmButtonText = "Aceptar",
                CancelButtonText = "Cancelar"
            });

            if (!string.IsNullOrEmpty(result.Value))
            {
                elimino = await productoServicio.Eliminar(prod.Codigo);

                if (elimino)
                {
                    await Swal.FireAsync("Felicidades", "Producto Eliminado", SweetAlertIcon.Success);
                    navigationManager.NavigateTo("/Productos");
                }
                else
                {
                    await Swal.FireAsync("Error", "No se Pudo Eliminar el Producto", SweetAlertIcon.Error);
                }
            }
        }
    }
}
