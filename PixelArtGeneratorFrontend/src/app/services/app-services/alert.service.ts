import { Injectable } from "@angular/core";
import { IndividualConfig, ToastrService } from "ngx-toastr";

@Injectable()
export class AlertService {
    constructor(private toastr: ToastrService) { }

    showError(message: string, title?: string, options?: Partial<IndividualConfig>)
    {
        this.toastr.error(message, title, options)
    }

    showSuccess(message: string, title?: string, options?: Partial<IndividualConfig>)
    {
        this.toastr.success(message, title, options)
    }

    showInfo(message: string, title?: string, options?: Partial<IndividualConfig>)
    {
        this.toastr.info(message, title, options)
    }
}