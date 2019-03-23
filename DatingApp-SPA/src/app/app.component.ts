import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  // when this component loads, this is the one we'll use to get the token from local storage.
  // if we have the token, then we will set decoded token in authservice to that token, but decoded.
  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      // when the app loads, the authservice is going to have decodedtoken, as long as the token is available in the local storage.
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
  }
}
