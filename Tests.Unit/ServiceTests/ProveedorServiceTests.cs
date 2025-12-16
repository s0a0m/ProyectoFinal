// using System.IO;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using Xunit;
// using Moq;
// using src.Core.Services.Implementations;
// using src.Contracts;
// using src.Repositories.Interfaces;
// using src.Models.Domain;

// public class ProductoProveedorServiceTests
// {
//     [Fact]
//     public async Task ProcesarLista_DeberiaClasificarFilasCorrectamente()
//     {
//         // ARRANGE ------------------------------------------------------------

//         // Datos simulados del Excel
//         var filasExcel = new List<ProductoProveedorDataRow>
//         {
//             new ProductoProveedorDataRow
//             {
//                 CodigoBarraExterno = "EXISTE-001",
//                 NombreSugerido = "Martillo",
//                 Precio = 1500,
//                 StockActual = 10
//             },
//             new ProductoProveedorDataRow
//             {
//                 CodigoBarraExterno = "NUEVO-002",
//                 NombreSugerido = "Llave Francesa",
//                 Precio = 2200,
//                 StockActual = 5
//             }
//         };

//         short proveedorId = 10;

//         // Mock Excel Reader
//         var excelMock = new Mock<IExcelDataReader>();
//         excelMock.Setup(x => x.ReadDataAsync(It.IsAny<Stream>(), It.IsAny<ImportacionColumnaMap>()))
//                  .ReturnsAsync(filasExcel);

//         // Mock Producto Existente
//         var productoAsociado = new Producto
//         {
//             IdProducto = 77,
//             Nombre = "Martillo Profesional"
//         };

//         var productoCodigoMock = new Mock<IProductoCodigoExternoRepository>();
//         productoCodigoMock.Setup(x => x.ObtenerProductoPorCodigoAsync("EXISTE-001", proveedorId))
//                           .ReturnsAsync(productoAsociado);

//         productoCodigoMock.Setup(x => x.ObtenerProductoPorCodigoAsync("NUEVO-002", proveedorId))
//                           .ReturnsAsync((Producto)null);

//         // Mock actualización de producto
//         var productoProveedorRepoMock = new Mock<IProductoProveedorRepository>();
//         productoProveedorRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ProductoProveedor>()))
//                                  .Returns(Task.CompletedTask);

//         // Mock novedades
//         var novedadesRepoMock = new Mock<INovedadesRepository>();
//         novedadesRepoMock.Setup(x => x.AddAsync(It.IsAny<NovedadPendiente>()))
//                          .Returns(Task.CompletedTask);

//         // Mock barcode adapter
//         var barcodeMock = new Mock<IBarcodeAdapter>();
//         barcodeMock.Setup(x => x.ValidarFormatoGS1EAN13(It.IsAny<string>()))
//                    .Returns(true);

//         // Repositorio no utilizado en esta prueba
//         var proveedorRepoMock = new Mock<IProveedorRepository>();
//         var commonDataMock = new Mock<ICommonDataService>();

//         // Crear servicio REAL
//         var service = new ProductoProveedorService(
//             excelMock.Object,
//             proveedorRepoMock.Object,
//             commonDataMock.Object,
//             productoCodigoMock.Object,
//             productoProveedorRepoMock.Object,
//             barcodeMock.Object,
//             novedadesRepoMock.Object
//         );

//         // ACT ------------------------------------------------------------
//         var resultados = await service.ProcesarListaDePreciosAsync(
//             new MemoryStream(),
//             proveedorId,
//             new ImportacionColumnaMap()
//         );

//         var lista = new List<AccionDeFilaCargaAutomatica>(resultados);

//         // ASSERT ------------------------------------------------------------

//         Assert.Equal(2, lista.Count);

//         // 1) Producto existente → actualización
//         var actualizacion = lista.Find(r => r.CodigoBarra == "EXISTE-001");
//         Assert.NotNull(actualizacion);
//         Assert.Equal("Actualización de Precio", actualizacion.Accion);

//         // 2) Producto nuevo → novedad
//         var novedad = lista.Find(r => r.CodigoBarra == "NUEVO-002");
//         Assert.NotNull(novedad);
//         Assert.Contains("Novedad Creada", novedad.Accion);

//         // Verifica que llamamos a UpdateAsync 1 vez
//         productoProveedorRepoMock.Verify(x => x.UpdateAsync(It.IsAny<ProductoProveedor>()), Times.Once);

//         // Verifica que se creó exactamente 1 novedad
//         novedadesRepoMock.Verify(x => x.AddAsync(It.IsAny<NovedadPendiente>()), Times.Once);
//     }
// }
