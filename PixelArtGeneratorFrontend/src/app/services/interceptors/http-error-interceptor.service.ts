import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, Observable, throwError } from "rxjs";
import { ConfigStore } from "src/app/app-config/config-store";
import { AlertService } from "../app-services/alert.service";

@Injectable()
export class HttpErrorInterceptorService implements HttpInterceptor{
    constructor( private router: Router,
        private alertService: AlertService,
        private configStore : ConfigStore) 
        {}

        intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
            return next.handle(req).pipe(
                catchError(error => {
                    if(error instanceof HttpErrorResponse) {
                        console.log(error);
                        if(error.error instanceof ErrorEvent || error.error instanceof ProgressEvent){
                            this.alertService.showError("Brak połączenia z serwerem")
                        } else {
                            switch(error.status){
                                case 400:
                                    this.alertService.showError(error.error)
                                    break;
                                case 401:
                                    this.alertService.showError("Użytkownik nie jest zautoryzowany")
                                    break;
                                case 403:
                                    this.alertService.showError("Brak dostępu do zasobu");
                                    this.router.navigateByUrl("");
                                    break;
                                case 404:
                                    this.alertService.showError("Nie znaleziono zasobu");
                                    break;
                                case 408:
                                    this.alertService.showError("Koniec czasu oczekiwania na żądanie");
                                    break;
                                case 500:
                                    this.alertService.showError("Wystąpił wewnętrzny błąd serwera, spróbuj ponownie za kilka minut");
                                    break;
                            }
                        }
                    }
                    this.configStore.stopLoadingPanel();
                   // return throwError(error.error);
                    return throwError(() => new Error(error.error))
                })
            )
        }
}