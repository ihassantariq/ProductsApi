export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  colour: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  colour: string;
}
