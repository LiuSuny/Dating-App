import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})

//ControlValueAccessor is use to create a reusable text input 
//and behaviour b/w angular form(reactiveform) and DOM e.g formControlName='confirmPassword' inside our register component
export class TextInputComponent implements ControlValueAccessor{

  @Input() label: string;
  @Input() labelMustMatch: string = '';
  @Input() placeHolder: string = '';
  @Input() type = 'text';
  

  //@Self() this provide us self contained and prevent angular fetching us addition instance
  //A base class that all FormControl-based directives extend. It binds a FormControl object to a DOM element. 
  constructor(@Self() public ngControl:NgControl ){
         this.ngControl.valueAccessor = this; //we get access to our control when call upon 
  }
  writeValue(obj: any): void {
    
  }

  registerOnChange(fn: any): void {
   
  }

  registerOnTouched(fn: any): void {
    
  }
  


}
