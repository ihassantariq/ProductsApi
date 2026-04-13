import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductService } from '../../services/product.service';
import { CreateProductRequest } from '../../models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  template: `
    <form (ngSubmit)="onSubmit()" class="product-form">
      <h3>Create New Product</h3>

      <mat-form-field appearance="outline">
        <mat-label>Name</mat-label>
        <input matInput [(ngModel)]="product.name" name="name" required />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Description</mat-label>
        <input matInput [(ngModel)]="product.description" name="description" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Price</mat-label>
        <input matInput type="number" [(ngModel)]="product.price" name="price" required min="0.01" step="0.01" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Colour</mat-label>
        <input matInput [(ngModel)]="product.colour" name="colour" required />
      </mat-form-field>

      <button mat-raised-button color="primary" type="submit" [disabled]="isLoading">
        {{ isLoading ? 'Creating...' : 'Create Product' }}
      </button>
    </form>
  `,
  styles: [`
    .product-form {
      display: flex;
      flex-direction: column;
      gap: 4px;
      max-width: 400px;
    }
    mat-form-field {
      width: 100%;
    }
  `]
})
export class ProductFormComponent {
  @Output() productCreated = new EventEmitter<void>();

  product: CreateProductRequest = { name: '', description: '', price: 0, colour: '' };
  isLoading = false;

  constructor(
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {}

  onSubmit(): void {
    this.isLoading = true;
    this.productService.createProduct(this.product).subscribe({
      next: () => {
        this.snackBar.open('Product created successfully.', 'Close', { duration: 3000 });
        this.product = { name: '', description: '', price: 0, colour: '' };
        this.isLoading = false;
        this.productCreated.emit();
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to create product.', 'Close', { duration: 3000 });
      }
    });
  }
}
