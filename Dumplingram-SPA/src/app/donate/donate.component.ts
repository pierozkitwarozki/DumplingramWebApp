import {
  AfterViewInit,
  Component,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { StripeCard, StripeSource, Token } from 'stripe-angular';
import { AlertifyService } from '../_services/alertify.service';
import { PaymentService } from '../_services/payment.service';
import { loadStripe } from '@stripe/stripe-js';

@Component({
  selector: 'app-donate',
  templateUrl: './donate.component.html',
  styleUrls: ['./donate.component.css'],
})
export class DonateComponent implements OnInit {
  stripePromise: any;
  session: any;
  @ViewChild('stripeCard') stripeCard: StripeCard;
  constructor(
    private alertify: AlertifyService,
    private paymentService: PaymentService
  ) {}

  ngOnInit() {
    this.stripePromise = loadStripe(
      'pk_test_51I5YvcJvKXNzTQcriYlZbz2AY6gfE97xSpJ7mUaL2UWOAABOjPBN9mlS3421iRf2nL25anPw6bgSSP5mxg7yI1Z600SJy8Gsm7'
    );
    debugger;
  }

  goToCheckout() {
    const x: any = { amount: 1000 };
    this.paymentService.charge(x).subscribe(
      async (data) => {
        this.session = data;
        const stripe = await this.stripePromise;
        const result = await stripe.redirectToCheckout({
          sessionId: this.session.id,
        });
        
        if(result.error) {
          this.alertify.error(result.error.message);
        } else {
          this.alertify.success('Sukces');
        }
      },
      (error) => this.alertify.error(error)
    );
  }
}
