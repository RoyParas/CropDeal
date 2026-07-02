import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { UserService } from '../../../services/user/user.service';
import { AddressService } from '../../../services/address/address.service';
import { BankAccountService } from '../../../services/bankAccount/bank-account.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-profile.component.html',
})
export class UserProfileComponent implements OnInit {
  profileForm!: FormGroup;
  addressForm!: FormGroup;
  bankForm!: FormGroup;

  role: string | null = null;
  isEditProfileMode = false;
  isEditAddressMode = false;
  isEditingBankInfo = false;

  isLoading = false;
  isSavingProfile = false;
  isSavingAddress = false;
  isSavingBank = false;

  hasAddress = false;
  hasBankInfo = false;
  showAddressForm = false;
  showBankForm = false;

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private addressService = inject(AddressService);
  private bankAccountService = inject(BankAccountService);

  ngOnInit(): void {
    this.initForms();
    this.role = this.authService.getRole();
    this.loadProfile();
  }

  private initForms(): void {
    this.profileForm = this.fb.group({
      fullName: [''],
      email: [''],
      phoneNumber: [''],
      averageRating: [''],
      role: ['']
    });

    this.addressForm = this.fb.group({
      state: [''],
      district: [''],
      city: [''],
      location: [''],
      zipCode: ['']
    });

    this.bankForm = this.fb.group({
      bankName: [''],
      branchName: [''],
      ifscCode: [''],
      accountNumber: ['']
    });
  }

  loadProfile(): void {
    this.isLoading = true;
    this.userService.getProfile().subscribe({
      next: (data) => {
        this.profileForm.patchValue(data);
        if (data.address) {
          this.hasAddress = true;
          this.addressForm.patchValue(data.address);
        }
        if (data.bankAccount) {
          this.hasBankInfo = true;
          this.bankForm.patchValue(data.bankAccount);
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.handleError(err);
        this.isLoading = false;
      }
    });
  }


  toggleProfileEdit(): void {
    this.isEditProfileMode = !this.isEditProfileMode;
    if (!this.isEditProfileMode) {
      this.isSavingProfile = true;
      this.userService.updateProfile(this.profileForm.value).subscribe({
        next: () => {
          this.isSavingProfile = false;
        },
        error: (err) => {
          this.handleError(err);
          this.isSavingProfile = false;
        }
      });
    }
  }


  toggleAddressEdit(): void {
    this.isEditAddressMode = !this.isEditAddressMode;
    if (!this.isEditAddressMode) {
      this.saveAddress();
    }
  }

  showAddress(): void {
    this.showAddressForm = true;
    this.isEditAddressMode = true;
  }

  saveAddress(): void {
    const address = this.addressForm.value;
    this.isSavingAddress = true;
    const action = this.hasAddress
      ? this.addressService.editAddress(address)
      : this.addressService.addAdress(address);

    action.subscribe({
      next: () => {
        this.hasAddress = true;
        this.isEditAddressMode = false;
        this.showAddressForm = false;
        this.isSavingAddress = false;
      },
      error: (err) => {
        this.handleError(err);
        this.isSavingAddress = false;
      }
    });
  }


  deleteAddress(): void {
    this.addressService.deleteAddress().subscribe({
      next: () => {
        this.addressForm.reset();
        this.hasAddress = false;
        this.isEditAddressMode = false;
      },
      error: this.handleError
    });
  }

  showBank(): void {
    this.showBankForm = true;
    this.isEditingBankInfo = true;
  }

  saveBankInfo(): void {
    const account = this.bankForm.value;
    this.isSavingBank = true;
    this.bankAccountService.addBankAccount(account).subscribe({
      next: () => {
        this.hasBankInfo = true;
        this.isEditingBankInfo = false;
        this.showBankForm = true;
        this.isSavingBank = false;
      },
      error: (err) => {
        this.handleError(err);
        this.isSavingBank = false;
      }
    });
  }


  deleteBankInfo(): void {
    this.bankAccountService.deleteBankAccount().subscribe({
      next: () => {
        this.bankForm.reset();
        this.hasBankInfo = false;
        this.showBankForm = false;
        this.isEditingBankInfo = false;
      },
      error: this.handleError
    });
  }

  navigateToChangePassword(): void {
    this.router.navigate(['/change-password']);
  }

  private handleError = (error: any): void => {
    console.log(error);
    const errorMessage = typeof error === 'object' ? error.error : error;
    console.error(errorMessage);
    // Ideally, show this in a toast/snackbar
  };
}
