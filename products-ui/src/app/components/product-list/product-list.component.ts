import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { ProductFormComponent } from '../product-form/product-form.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatToolbarModule,
    MatCardModule,
    ProductFormComponent
  ],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  selectedColour = '';
  colours = ['Red', 'Blue', 'Green', 'Black', 'White', 'Orange', 'Purple', 'Yellow'];
  displayedColumns = ['id', 'name', 'description', 'price', 'colour'];

  constructor(
    private productService: ProductService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    const colour = this.selectedColour || undefined;
    this.productService.getAllProducts(colour).subscribe({
      next: (products) => this.products = products,
      error: () => this.products = []
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
