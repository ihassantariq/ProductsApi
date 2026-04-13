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
  template: `
    <mat-toolbar color="primary">
      <span>Products API</span>
      <span class="spacer"></span>
      <button mat-button (click)="onLogout()">Logout</button>
    </mat-toolbar>

    <div class="content">
      <div class="top-section">
        <mat-card class="form-card">
          <mat-card-content>
            <app-product-form (productCreated)="loadProducts()"></app-product-form>
          </mat-card-content>
        </mat-card>

        <mat-card class="filter-card">
          <mat-card-content>
            <h3>Filter Products</h3>
            <mat-form-field appearance="outline">
              <mat-label>Filter by Colour</mat-label>
              <mat-select [(ngModel)]="selectedColour" (selectionChange)="loadProducts()">
                <mat-option value="">All Colours</mat-option>
                <mat-option *ngFor="let colour of colours" [value]="colour">{{ colour }}</mat-option>
              </mat-select>
            </mat-form-field>
          </mat-card-content>
        </mat-card>
      </div>

      <mat-card class="table-card">
        <mat-card-content>
          <h3>Products ({{ products.length }})</h3>
          <table mat-table [dataSource]="products" class="full-width">
            <ng-container matColumnDef="id">
              <th mat-header-cell *matHeaderCellDef>ID</th>
              <td mat-cell *matCellDef="let product">{{ product.id }}</td>
            </ng-container>

            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let product">{{ product.name }}</td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let product">{{ product.description }}</td>
            </ng-container>

            <ng-container matColumnDef="price">
              <th mat-header-cell *matHeaderCellDef>Price</th>
              <td mat-cell *matCellDef="let product">{{ product.price | currency }}</td>
            </ng-container>

            <ng-container matColumnDef="colour">
              <th mat-header-cell *matHeaderCellDef>Colour</th>
              <td mat-cell *matCellDef="let product">{{ product.colour }}</td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .spacer {
      flex: 1 1 auto;
    }
    .content {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }
    .top-section {
      display: flex;
      gap: 24px;
      margin-bottom: 24px;
      flex-wrap: wrap;
    }
    .form-card, .filter-card {
      flex: 1;
      min-width: 300px;
    }
    .table-card {
      width: 100%;
    }
    .full-width {
      width: 100%;
    }
    table {
      width: 100%;
    }
  `]
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
