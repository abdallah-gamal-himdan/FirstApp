import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  regForm :FormGroup;
  maxDate:Date;
  validationErrors:string[] =[];

  constructor(private accountService: AccountService, private toastr: ToastrService,private Fb : FormBuilder
    ,private router:Router) { }

  ngOnInit(): void {
    this.initForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
  initForm()
  {
    this.regForm = this.Fb.group(
      {
        username :  [null,[Validators.required]],
        gender :  ['male'],
        KnownAs :  [null,[Validators.required]],
        dateOfBirth :  [null,[Validators.required]],
        city :  [null,[Validators.required]],
        country :  [null,[Validators.required]],
        password :[null,[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
        confirmPassword:[null,[Validators.required,Validators.minLength(4),
          this.matchePassword('password')]]
      }
    );
    this.regForm.controls.password.valueChanges.subscribe(()=>
    {
      this.regForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  matchePassword(matchTo:string) : ValidatorFn
  {
    return (control : AbstractControl)=>
    {
      return control?.value === control?.parent?.controls[matchTo].value ? 
      null : {isNotMatching :true};
    }    
  }

  register() {
      this.accountService.register(this.regForm.value).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      this.validationErrors =error;
    })

    console.log(this.regForm.value);
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
