import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://kadir-api-a4eeeaeygvdxage6.canadaeast-01.azurewebsites.net/api',
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true // Refresh token cookie'si için gerekli
});

// Request Interceptor: JWT Ekleme
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor for error handling
api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    const token = localStorage.getItem('token');
    
    // Eğer istek hata loglama endpoint'ine atıldıysa sonsuz döngüyü kırmak için yönlendirme yapma
    if (originalRequest.url.includes('/Analytics/log-error')) {
      return Promise.reject(error);
    }
    
    // Eğer 401 (Token Süresi Doldu) ise zaten aşağıda sessizce yenileyeceğiz
    // Güvenlik: Üretim ortamında konsola hata basmıyoruz
    // if (!error.response || error.response.status !== 401) {
    //   console.error('API Error:', error.response ? JSON.stringify(error.response.data) : error.message);
    // }
    
    // Silent Refresh Mantığı (401 Alındığında)
    if (error.response && error.response.status === 401 && !originalRequest._retry && token) {
      originalRequest._retry = true;
      try {
        const refreshUrl = (import.meta.env.VITE_API_URL || 'https://kadir-api-a4eeeaeygvdxage6.canadaeast-01.azurewebsites.net/api') + '/auth/refresh';
        const res = await axios.post(refreshUrl, {}, { 
          withCredentials: true,
          headers: { Authorization: `Bearer ${token}` }
        });
        if (res.data.success) {
          localStorage.setItem('token', res.data.token);
          originalRequest.headers.Authorization = `Bearer ${res.data.token}`;
          return api(originalRequest); // Yarım kalan isteği yeni token ile tekrarla
        }
      } catch (refreshError) {
        localStorage.removeItem('token');
        // Sadece kullanıcı gerçekten admin rotasındaysa giriş ekranına yönlendir
        if (window.location.pathname.startsWith('/admin')) {
          window.location.href = '/admin/login';
        }
        return Promise.reject(refreshError);
      }
    }
    
    // API hatalarını backend Sistem Sağlığı (Health Logs) bölümüne gönder
    if (token && error.config && !error.config.url.includes('/Analytics/log-error')) {
      api.post('/Analytics/log-error', {
        errorType: 'API_ERROR',
        details: `Endpoint: ${error.config.url} | Message: ${error.message}`
      }).catch(() => { /* Loglama sırasında hata olursa yut */ });
    }
    
    return Promise.reject(error);
  }
);

export default api;
