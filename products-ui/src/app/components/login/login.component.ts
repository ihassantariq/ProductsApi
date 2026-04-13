import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginRequest: LoginRequest = { username: '', password: '' };
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onLogin(): void {
    this.isLoading = true;
    this.authService.login(this.loginRequest).subscribe({
      next: () => {
        this.router.navigate(['/products']);
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Invalid username or password.', 'Close', { duration: 3000 });
      }
    });
  }
}
