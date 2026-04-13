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
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
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
