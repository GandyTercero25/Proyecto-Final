import React, { useEffect, useState } from 'react';

export default function ProductosDestacados() {
    const [productos, setProductos] = useState([]);
    const [cargando, setCargando] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch('/api/productos')
            .then(res => {
                if (!res.ok) throw new Error('Error al cargar productos');
                return res.json();
            })
            .then(data => {
                setProductos(data);
                setCargando(false);
            })
            .catch(err => {
                setError(err.message);
                setCargando(false);
            });
    }, []);

    if (cargando) return <div className="text-center mt-5">Cargando productos...</div>;
    if (error) return <div className="text-danger text-center mt-5">Error: {error}</div>;

    return (
        <div className="row">
            {productos.map(p => (
                <div className="col-md-4 mb-4" key={p.productoId}>
                    <div className="card h-100 shadow-sm border-0">
                        <img
                            src={p.imagenUrl || "https://via.placeholder.com/300"}
                            className="card-img-top"
                            alt={p.nombre}
                            style={{ height: '200px', objectFit: 'cover' }}
                        />
                        <div className="card-body d-flex flex-column">
                            <h5 className="card-title">{p.nombre}</h5>
                            <p className="card-text text-muted">{p.descripcion}</p>
                            <div className="mt-auto">
                                <p className="fw-bold text-success">${p.precio.toFixed(2)}</p>
                                <a href={`/Productos/Details/${p.productoId}`} className="btn btn-primary btn-sm">Ver más</a>
                            </div>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}
