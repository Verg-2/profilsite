import api from './api';

const translationService = {
  translate: async (text, targetLanguage = 'English', section = 'Genel') => {
    if (!text) return '';
    try {
      const response = await api.post('/Translation/Translate', {
        text,
        targetLanguage,
        section
      });
      return response.data;
    } catch (error) {
      console.error('Translation error:', error);
      throw error;
    }
  },
  refine: async (text, existingTranslation, targetLanguage = 'English', section = 'Genel', userHint = null) => {
    try {
      const payload = {
        text,
        existingTranslation,
        targetLanguage,
        section
      };
      if (userHint) {
        payload.userHint = userHint;
      }
      const response = await api.post('/Translation/Refine', payload);
      return response.data;
    } catch (error) {
      console.error('Refine Translation error:', error);
      throw error;
    }
  }
};

export default translationService;
