import React from 'react';
import { createRoot } from 'react-dom/client';
import ProductosDestacados from './ProductosDestacados';

const root = createRoot(document.getElementById('react-productos'));
root.render(<ProductosDestacados />);
