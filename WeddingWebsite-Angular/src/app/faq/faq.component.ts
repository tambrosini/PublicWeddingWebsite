import { Component, OnInit } from '@angular/core';
import { Faq } from '../models/faq-model';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq',
  imports: [CommonModule],
  templateUrl: './faq.component.html',
  styleUrl: './faq.component.scss'
})
export class FaqComponent implements OnInit{

  private readonly JSON_URL = 'assets/faq.json';
  faqs: Faq[] = [];

  constructor(private httpClient: HttpClient) { 
  }

  ngOnInit(): void {
    this.loadFaqs();
  }


  loadFaqs(): void {
    this.httpClient.get<Faq[]>(this.JSON_URL).subscribe({
      next: (data) => {
        this.faqs = data;
        console.log('FAQs loaded successfully', this.faqs);
      },
      error: (error) => {
        console.error('Error loading FAQs', error);
      }
    });
  }
}
