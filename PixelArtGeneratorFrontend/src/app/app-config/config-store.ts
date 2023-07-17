import { Subject } from "rxjs";

export class ConfigStore {
    loadingPanel: Subject<boolean> = new Subject<boolean>();

    startLoadingPanel() {
        this.loadingPanel.next(true);
    }
    stopLoadingPanel() {
        this.loadingPanel.next(false);
    }
}