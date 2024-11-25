import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  
  //@Input() usersFromHomeComponent : any;
  @Output() cancelRegister = new EventEmitter();

  //model: any = {};
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  //make use of private formBuilder: FormBuilder to build our form as it 
  // provides syntactic sugar that shortens creating instances of a FormControl , FormGroup , or FormArray . It reduces the amount of boilerplate
  constructor(private accountService: AccountService, 
    private toastr: ToastrService, private formBuilder: FormBuilder,
    private router: Router
  ){}

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
 
  initializeForm(){
     this.registerForm = this.formBuilder.group({
      gender : ['male'],
      username : ['', Validators.required],
      knownAs : ['', Validators.required],
      dateOfBirth : ['', Validators.required],
      city : ['', Validators.required],
      country : ['', Validators.required],
      password :['', [Validators.required,
        Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
     })
     //checking if there is changes in password field

     //checking if there is changes in password field
    this.registerForm.controls[' password'].valueChanges.subscribe(() =>{
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }

////initially used for formgroup
//   initializeForm(){
//     this.registerForm = new FormGroup({
//      username : new FormControl('', Validators.required),
//      password : new FormControl(
//        '', [Validators.required,
//           Validators.minLength(4), Validators.maxLength(8)]
//      ),
//      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
//     })
//     //checking if there is changes in password field

//     //checking if there is changes in password field
//    this.registerForm.controls[' password'].valueChanges.subscribe(() =>{
//      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
//    })
//  }

  //match value to confirm if the password and confrim password matches
   
  matchValues(matchTo : string) : ValidatorFn{
      return (control: AbstractControl) => {
               //control?.value--compare this to control?.parent? if the matchto is the same or not
               //which is password -(parent) control?value(child) confirmpassword
        return control?.value === control?.parent?.controls[matchTo].value 
        ? null : {isMatching: true}
      }
    }

   register(){
    this.accountService.register(this.registerForm.value).subscribe(response =>{    
      this.router.navigateByUrl('/members');
    }, error=>{
      this.validationErrors = error;
      //this.toastr.error(error.error);
    });
    
   }

   cancel(){
    this.cancelRegister.emit(false);
   }
}
