import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shortenId',
})
export class ShortenIdPipe implements PipeTransform {
  transform(value: string, length: number = 12): string {
    if (!value) return '';
    if (value.length <= length) return value;
    return `${value.substring(0, 6)}...${value.substring(value.length - 6)}`;
  }
}