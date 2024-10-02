import { HttpClient, HttpStatusCode } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit{

  baseURL= 'https://localhost:5000/api/';
  validationErrors: string[] = [];

  constructor(private httpclient :HttpClient) {}
 
  ngOnInit(): void {
   
  }

  get404Errors(){
    this.httpclient.get(this.baseURL + 'buggy/not-found').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get500Errors(){
    this.httpclient.get(this.baseURL + 'buggy/server-error').subscribe(response =>{
      console.log(response);
    }, error=> {
      console.log(error);
    })
  }

  get400Errors(){
    this.httpclient.get(this.baseURL + 'buggy/bad-request').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get401Errors(){
    this.httpclient.get(this.baseURL + 'buggy/auth').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get400ValidationErrors(){
    this.httpclient.post(this.baseURL + 'account/register', {}).subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
      this.validationErrors = error;
    })
  }
}
