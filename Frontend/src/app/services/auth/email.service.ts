import { Injectable } from '@angular/core';
import emailjs from '@emailjs/browser';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmailService {

  private serviceID = environment.emailjs_serviceID;
  private templateID = environment.emailjs_templateID;
  private publicKey = environment.emailjs_publicKey;

  constructor() { }

  sendPasswordResetEmail(toEmail: string, resetLink: string, toName: string) {
    const templateParams = {
      to_email: toEmail,
      to_name: toName,
      reset_link: resetLink
    };

    return emailjs.send(this.serviceID, this.templateID, templateParams, this.publicKey);
  }
}
