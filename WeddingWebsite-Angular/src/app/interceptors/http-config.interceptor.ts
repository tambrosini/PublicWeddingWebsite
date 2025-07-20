import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const httpConfigInterceptor: HttpInterceptorFn = (req, next) => {
  // Clone the request and add withCredentials
  const modifiedReq = req.clone({
    withCredentials: true
  });

  // Return next handler with error handling
  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Log each validation error in a readable format
      if (error.error?.errors && typeof error.error.errors === 'object') {
        const validationErrors = error.error.errors;
        for (const key in validationErrors) {
          if (validationErrors.hasOwnProperty(key)) {
            const messages: string[] = validationErrors[key];
            if (Array.isArray(messages)) {
              messages.forEach((msg) => {
                console.error(`${key}: "${msg}"`);
              });
            }
          }
        }
      } else {
        console.error('HTTP request failed:', error);
      }

      // Handle authentication errors
      if (error.status === 401 || error.status === 403) {
        console.warn('Authentication error - redirecting to login');
        // Add login redirection if needed
      }

      return throwError(() => error);
    })
  );
};