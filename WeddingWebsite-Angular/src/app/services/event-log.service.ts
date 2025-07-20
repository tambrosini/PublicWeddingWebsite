import { Injectable } from '@angular/core';
import { ApiUrl } from '../constants';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { EventLogModel } from '../models/event-log-model';
import { JsonPreserveWrapper } from '../models/json-preserve-wrapper';

@Injectable({
  providedIn: 'root'
})
export class EventLogService {

  private apiUrl = `${ApiUrl}/logs`;

  constructor(private http: HttpClient) { }

  getLogs(): Observable<EventLogModel[]> {
    return this.http.get<JsonPreserveWrapper<EventLogModel>>(this.apiUrl)
      .pipe(
        map(res => res.$values)
      );
  }
}
