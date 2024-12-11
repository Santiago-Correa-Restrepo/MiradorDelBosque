using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ultima.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosReserva",
                columns: table => new
                {
                    IdEstadoReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEstadoReserva = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EstadosR__3E654CA8AC5B1510", x => x.IdEstadoReserva);
                });

            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    IdImagen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imagenes__B42D8F2A3284391F", x => x.IdImagen);
                });

            migrationBuilder.CreateTable(
                name: "MetodoPago",
                columns: table => new
                {
                    IdMetodoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomMetodoPago = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MetodoPa__6F49A9BE0163EA2A", x => x.IdMetodoPago);
                });

            migrationBuilder.CreateTable(
                name: "Muebles",
                columns: table => new
                {
                    IdMueble = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Muebles__829D15E8CA56FBC7", x => x.IdMueble);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomPermiso = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PermisosSeleccionados = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "Sin permiso"),
                    Estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permisos__0D626EC87506DF0F", x => x.IdPermiso);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomRol = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__2A49584C4358CBA4", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                columns: table => new
                {
                    IdTipoDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomTipoDcumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoDocu__3AB3332F3F8876BE", x => x.IdTipoDocumento);
                });

            migrationBuilder.CreateTable(
                name: "TipoHabitaciones",
                columns: table => new
                {
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomTipoHabitacion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    NumeroPersonas = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoHabi__AB75E87C4FADDF8A", x => x.IdTipoHabitacion);
                });

            migrationBuilder.CreateTable(
                name: "TipoServicios",
                columns: table => new
                {
                    IdTipoServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoServicio = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoServ__E29B3EA7DE3B5020", x => x.IdTipoServicio);
                });

            migrationBuilder.CreateTable(
                name: "PermisosRoles",
                columns: table => new
                {
                    IdPermisosRoles = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRol = table.Column<int>(type: "int", nullable: true),
                    IdPermiso = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permisos__8C257ED9F16A96CD", x => x.IdPermisosRoles);
                    table.ForeignKey(
                        name: "FK__PermisosR__IdPer__18EBB532",
                        column: x => x.IdPermiso,
                        principalTable: "Permisos",
                        principalColumn: "IdPermiso");
                    table.ForeignKey(
                        name: "FK__PermisosR__IdRol__19DFD96B",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    NroDocumento = table.Column<int>(type: "int", nullable: false),
                    Tipo_Documento = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    IdTipoDocumento = table.Column<int>(type: "int", nullable: true),
                    Nombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Apellidos = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Celular = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Contrasena = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IdRol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__CC62C91CD8DABF24", x => x.NroDocumento);
                    table.ForeignKey(
                        name: "FK__Usuarios__IdRol__1F98B2C1",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    NroDocumento = table.Column<int>(type: "int", nullable: false),
                    IdTipoDocumento = table.Column<int>(type: "int", nullable: false),
                    Nombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Celular = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IdRol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clientes__CC62C91CB58EF0F1", x => x.NroDocumento);
                    table.ForeignKey(
                        name: "FK__Clientes__IdRol__05D8E0BE",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                    table.ForeignKey(
                        name: "FK__Clientes__IdTipo__06CD04F7",
                        column: x => x.IdTipoDocumento,
                        principalTable: "TipoDocumento",
                        principalColumn: "IdTipoDocumento");
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Costo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__8BBBF90102DDD785", x => x.IdHabitacion);
                    table.ForeignKey(
                        name: "FK__Habitacio__IdTip__0B91BA14",
                        column: x => x.IdTipoHabitacion,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "IdTipoHabitacion");
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTipoServicio = table.Column<int>(type: "int", nullable: false),
                    NomServicio = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Servicio__2DCCF9A24485B156", x => x.IdServicio);
                    table.ForeignKey(
                        name: "FK__Servicios__IdTip__1EA48E88",
                        column: x => x.IdTipoServicio,
                        principalTable: "TipoServicios",
                        principalColumn: "IdTipoServicio");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroDocumentoCliente = table.Column<int>(type: "int", nullable: false),
                    NroDocumentoUsuario = table.Column<int>(type: "int", nullable: true),
                    FechaReserva = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    SubTotal = table.Column<double>(type: "float", nullable: true),
                    Descuento = table.Column<double>(type: "float", nullable: true),
                    IVA = table.Column<double>(type: "float", nullable: true),
                    MontoTotal = table.Column<double>(type: "float", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false),
                    NroPersonas = table.Column<int>(type: "int", nullable: true),
                    IdEstadoReserva = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservas__0E49C69D3D983CC1", x => x.IdReserva);
                    table.ForeignKey(
                        name: "FK__Reservas__IdEsta__1AD3FDA4",
                        column: x => x.IdEstadoReserva,
                        principalTable: "EstadosReserva",
                        principalColumn: "IdEstadoReserva");
                    table.ForeignKey(
                        name: "FK__Reservas__Metodo__1BC821DD",
                        column: x => x.MetodoPago,
                        principalTable: "MetodoPago",
                        principalColumn: "IdMetodoPago",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Reservas__NroDoc__1CBC4616",
                        column: x => x.NroDocumentoCliente,
                        principalTable: "Clientes",
                        principalColumn: "NroDocumento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Reservas__NroDoc__1DB06A4F",
                        column: x => x.NroDocumentoUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "NroDocumento");
                });

            migrationBuilder.CreateTable(
                name: "HabitacionMueble",
                columns: table => new
                {
                    IdHabitacionMueble = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false),
                    IdMueble = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__402E049D3966CCDF", x => x.IdHabitacionMueble);
                    table.ForeignKey(
                        name: "FK__Habitacio__IdHab__0C85DE4D",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion");
                    table.ForeignKey(
                        name: "FK__Habitacio__IdMue__0D7A0286",
                        column: x => x.IdMueble,
                        principalTable: "Muebles",
                        principalColumn: "IdMueble");
                });

            migrationBuilder.CreateTable(
                name: "ImagenHabitacion",
                columns: table => new
                {
                    IdImagenHabi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImagen = table.Column<int>(type: "int", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImagenHa__5B5FF6AD92383AF0", x => x.IdImagenHabi);
                    table.ForeignKey(
                        name: "FK__ImagenHab__IdHab__10566F31",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion");
                    table.ForeignKey(
                        name: "FK__ImagenHab__IdIma__114A936A",
                        column: x => x.IdImagen,
                        principalTable: "Imagenes",
                        principalColumn: "IdImagen");
                });

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    IdPaquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomPaquete = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquetes__DE278F8BD7FB8643", x => x.IdPaquete);
                    table.ForeignKey(
                        name: "FK__Paquetes__IdHabi__160F4887",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagenServicio",
                columns: table => new
                {
                    IdImagenServi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImagen = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImagenSe__3C03784C1C47D76A", x => x.IdImagenServi);
                    table.ForeignKey(
                        name: "FK__ImagenSer__IdIma__14270015",
                        column: x => x.IdImagen,
                        principalTable: "Imagenes",
                        principalColumn: "IdImagen");
                    table.ForeignKey(
                        name: "FK__ImagenSer__IdSer__151B244E",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateTable(
                name: "Abono",
                columns: table => new
                {
                    IdAbono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: true),
                    FechaAbono = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    ValorDeuda = table.Column<double>(type: "float", nullable: true),
                    Porcentaje = table.Column<double>(type: "float", nullable: true),
                    Pendiente = table.Column<double>(type: "float", nullable: true),
                    SubTotal = table.Column<double>(type: "float", nullable: false),
                    IVA = table.Column<double>(type: "float", nullable: true),
                    CantAbono = table.Column<double>(type: "float", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Abono__A4693DA717B60198", x => x.IdAbono);
                    table.ForeignKey(
                        name: "FK__Abono__IdReserva__04E4BC85",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                });

            migrationBuilder.CreateTable(
                name: "DetalleReservaServicio",
                columns: table => new
                {
                    IdDetalleReservaServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdServicio = table.Column<int>(type: "int", nullable: true),
                    IdReserva = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Costo = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleR__D3D91A5ABFAB516A", x => x.IdDetalleReservaServicio);
                    table.ForeignKey(
                        name: "FK__DetalleRe__IdRes__09A971A2",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                    table.ForeignKey(
                        name: "FK__DetalleRe__IdSer__0A9D95DB",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateTable(
                name: "DetalleReservaPaquete",
                columns: table => new
                {
                    DetalleReservaPaquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaquete = table.Column<int>(type: "int", nullable: true),
                    IdReserva = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Costo = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleR__2E8BFF25D36038AD", x => x.DetalleReservaPaquete);
                    table.ForeignKey(
                        name: "FK__DetalleRe__IdPaq__07C12930",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                    table.ForeignKey(
                        name: "FK__DetalleRe__IdRes__08B54D69",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                });

            migrationBuilder.CreateTable(
                name: "ImagenPaquete",
                columns: table => new
                {
                    IdImagenPaque = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImagen = table.Column<int>(type: "int", nullable: false),
                    IdPaquete = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImagenPa__AD53DF943757EF55", x => x.IdImagenPaque);
                    table.ForeignKey(
                        name: "FK__ImagenPaq__IdIma__123EB7A3",
                        column: x => x.IdImagen,
                        principalTable: "Imagenes",
                        principalColumn: "IdImagen");
                    table.ForeignKey(
                        name: "FK__ImagenPaq__IdPaq__1332DBDC",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                });

            migrationBuilder.CreateTable(
                name: "PaqueteServicio",
                columns: table => new
                {
                    IdPaqueteServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaquete = table.Column<int>(type: "int", nullable: true),
                    IdServicio = table.Column<int>(type: "int", nullable: true),
                    Costo = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaqueteS__DE5C2BC24D782718", x => x.IdPaqueteServicio);
                    table.ForeignKey(
                        name: "FK__PaqueteSe__IdPaq__17036CC0",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                    table.ForeignKey(
                        name: "FK__PaqueteSe__IdSer__17F790F9",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateTable(
                name: "ImagenAbono",
                columns: table => new
                {
                    IdImagenAbono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImagen = table.Column<int>(type: "int", nullable: true),
                    IdAbono = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImagenAb__A40FAE5309D1F3BF", x => x.IdImagenAbono);
                    table.ForeignKey(
                        name: "FK__ImagenAbo__IdAbo__0E6E26BF",
                        column: x => x.IdAbono,
                        principalTable: "Abono",
                        principalColumn: "IdAbono");
                    table.ForeignKey(
                        name: "FK__ImagenAbo__IdIma__0F624AF8",
                        column: x => x.IdImagen,
                        principalTable: "Imagenes",
                        principalColumn: "IdImagen");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abono_IdReserva",
                table: "Abono",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdRol",
                table: "Clientes",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdTipoDocumento",
                table: "Clientes",
                column: "IdTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReservaPaquete_IdPaquete",
                table: "DetalleReservaPaquete",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReservaPaquete_IdReserva",
                table: "DetalleReservaPaquete",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReservaServicio_IdReserva",
                table: "DetalleReservaServicio",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReservaServicio_IdServicio",
                table: "DetalleReservaServicio",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_IdTipoHabitacion",
                table: "Habitaciones",
                column: "IdTipoHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionMueble_IdHabitacion",
                table: "HabitacionMueble",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionMueble_IdMueble",
                table: "HabitacionMueble",
                column: "IdMueble");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenAbono_IdAbono",
                table: "ImagenAbono",
                column: "IdAbono");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenAbono_IdImagen",
                table: "ImagenAbono",
                column: "IdImagen");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenHabitacion_IdHabitacion",
                table: "ImagenHabitacion",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenHabitacion_IdImagen",
                table: "ImagenHabitacion",
                column: "IdImagen");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenPaquete_IdImagen",
                table: "ImagenPaquete",
                column: "IdImagen");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenPaquete_IdPaquete",
                table: "ImagenPaquete",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenServicio_IdImagen",
                table: "ImagenServicio",
                column: "IdImagen");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenServicio_IdServicio",
                table: "ImagenServicio",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_IdHabitacion",
                table: "Paquetes",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_PaqueteServicio_IdPaquete",
                table: "PaqueteServicio",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_PaqueteServicio_IdServicio",
                table: "PaqueteServicio",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_PermisosRoles_IdPermiso",
                table: "PermisosRoles",
                column: "IdPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_PermisosRoles_IdRol",
                table: "PermisosRoles",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdEstadoReserva",
                table: "Reservas",
                column: "IdEstadoReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_MetodoPago",
                table: "Reservas",
                column: "MetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_NroDocumentoCliente",
                table: "Reservas",
                column: "NroDocumentoCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_NroDocumentoUsuario",
                table: "Reservas",
                column: "NroDocumentoUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_IdTipoServicio",
                table: "Servicios",
                column: "IdTipoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleReservaPaquete");

            migrationBuilder.DropTable(
                name: "DetalleReservaServicio");

            migrationBuilder.DropTable(
                name: "HabitacionMueble");

            migrationBuilder.DropTable(
                name: "ImagenAbono");

            migrationBuilder.DropTable(
                name: "ImagenHabitacion");

            migrationBuilder.DropTable(
                name: "ImagenPaquete");

            migrationBuilder.DropTable(
                name: "ImagenServicio");

            migrationBuilder.DropTable(
                name: "PaqueteServicio");

            migrationBuilder.DropTable(
                name: "PermisosRoles");

            migrationBuilder.DropTable(
                name: "Muebles");

            migrationBuilder.DropTable(
                name: "Abono");

            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "TipoServicios");

            migrationBuilder.DropTable(
                name: "EstadosReserva");

            migrationBuilder.DropTable(
                name: "MetodoPago");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "TipoHabitaciones");

            migrationBuilder.DropTable(
                name: "TipoDocumento");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
