import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, CreateProductRequest } from '../models/product.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = `${environment.apiUrl}/api/Products`;

  constructor(private http: HttpClient) {}

  getAllProducts(colour?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (colour) {
      params = params.set('colour', colour);
    }
    return this.http.get<Product[]>(`${this.apiUrl}/GetAllProducts`, { params });
  }

  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/CreateProduct`, request);
  }
}
