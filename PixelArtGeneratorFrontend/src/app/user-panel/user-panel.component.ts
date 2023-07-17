import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigStore } from 'src/app/app-config/config-store'
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-user-panel',
  templateUrl: './user-panel.component.html',
  styleUrls: ['./user-panel.component.scss']
})
export class UserPanelComponent implements OnInit {

  selectedFile: File | null = null;
  processedImage: string | null = null;
  width: number | undefined;
  height: number | undefined;
  colorPalette: string | undefined;
  option: number | undefined;

  constructor(private http: HttpClient, private configStore: ConfigStore,) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }
  
  
  async uploadImage() {

    this.configStore.startLoadingPanel();
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('imageFile', this.selectedFile);
      formData.append('width', String(this.width));
      formData.append('height', String(this.height));
      formData.append('colorPalette', this.colorPalette || '');
      formData.append('option', String(this.option));

      await lastValueFrom(this.http.post('https://localhost:7004/api/pixel', formData, {responseType: 'blob'}))
        .then(
          response => {
            const reader = new FileReader();
            reader.onloadend = () => {
              this.processedImage = reader.result as string;
            };
            reader.readAsDataURL(response);
          },
          error => {
            this.configStore.stopLoadingPanel();
            console.error(error);
          }
        );
        this.configStore.stopLoadingPanel();
    }
  }

  downloadImage() {
    if (this.processedImage) {
      const link = document.createElement('a');
      link.href = this.processedImage;
      link.download = 'PixelArt.png';
      link.click();
    }
  }

  ngOnInit(): void {
   
  }
}
