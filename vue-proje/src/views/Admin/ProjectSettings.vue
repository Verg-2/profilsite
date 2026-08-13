<template>
  <div class="admin-page-wrapper">
    <div v-if="!isEditing" class="project-list-view">
      <div class="admin-page-header">
        <div>
          <h2 class="admin-title">Projeler Yönetimi</h2>
          <p class="admin-subtitle">Portfolyonuzdaki projeleri ve kategorileri düzenleyin.</p>
        </div>
        <div style="display: flex; gap: 1rem;">
          <button @click="openTrash" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
            <i class="fas fa-trash-restore"></i> Çöp Kutusu
          </button>
          <button @click="showCategoryModal = true" class="admin-btn admin-btn-secondary">
            <i class="fas fa-tags"></i> Kategoriler
          </button>
          <button @click="openEditor(null)" class="admin-btn admin-btn-primary">
            <i class="fas fa-plus"></i> Yeni Proje
          </button>
        </div>
      </div>

      <div v-if="errorMsg" style="background: rgba(239, 68, 68, 0.1); border: 1px solid var(--admin-danger); color: var(--admin-danger); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        <i class="fas fa-exclamation-circle"></i> {{ errorMsg }}
      </div>
      
      <div v-if="successMsg" style="background: rgba(16, 185, 129, 0.1); border: 1px solid var(--admin-success); color: var(--admin-success); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        <i class="fas fa-check-circle"></i> {{ successMsg }}
      </div>

      <div v-if="loading" style="text-align: center; padding: 3rem; color: var(--admin-primary);">
        <i class="fas fa-spinner fa-spin fa-2x"></i>
        <p style="margin-top: 1rem;">Projeler yükleniyor...</p>
      </div>

      <div v-else-if="projects.length === 0" style="text-align: center; padding: 3rem; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: var(--admin-radius-lg);">
        <i class="fas fa-project-diagram" style="font-size: 3rem; color: var(--admin-text-muted); margin-bottom: 1rem;"></i>
        <p style="color: var(--admin-text-muted);">Henüz hiç proje eklemediniz.</p>
      </div>

      <div v-else style="display: grid; grid-template-columns: repeat(auto-fill, minmax(min(100%, 320px), 1fr)); gap: 1.5rem;">
        <div v-for="project in projects" :key="project.id" class="admin-card" style="padding: 0; display: flex; flex-direction: column;">
          
          <div style="height: 180px; background: var(--admin-surface-hover); border-bottom: 1px solid var(--admin-border); overflow: hidden;">
            <img v-if="project.imageUrls && project.imageUrls.length > 0" :src="getFullUrl(project.imageUrls[0])" style="width: 100%; height: 100%; object-fit: cover;" />
            <div v-else style="display:flex; align-items:center; justify-content:center; height:100%;">
              <i class="fas fa-image" style="font-size: 4rem; color: var(--admin-text-muted);"></i>
            </div>
          </div>

          <div style="padding: 1.5rem; flex: 1; display: flex; flex-direction: column;">
            <div style="display: flex; justify-content: space-between; font-size: 0.8rem; color: var(--admin-text-muted); margin-bottom: 1rem;">
              <span style="background: rgba(255,51,0,0.1); color: var(--admin-primary); padding: 0.2rem 0.5rem; border-radius: 4px; border: 1px solid rgba(255,51,0,0.2);">
                {{ project.category?.name || 'Kategori Yok' }}
              </span>
            </div>
            
            <h3 style="font-size: 1.2rem; color: var(--admin-heading); margin-bottom: 0.75rem;">{{ project.title }}</h3>
            <p style="color: var(--admin-text-muted); font-size: 0.9rem; line-height: 1.5; margin-bottom: 1.5rem; flex: 1;">
              {{ project.summary }}
            </p>
            
            <div style="display: flex; justify-content: flex-end; gap: 0.5rem; border-top: 1px solid var(--admin-border); padding-top: 1rem;">
              <button @click="openEditor(project)" class="admin-btn admin-btn-secondary" style="padding: 0.5rem 1rem; font-size: 0.85rem;">
                <i class="fas fa-pen"></i> Düzenle
              </button>
              <button @click="deleteProject(project.id)" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2); padding: 0.5rem 1rem; font-size: 0.85rem;">
                <i class="fas fa-trash"></i> Sil
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit/Create Form -->
    <div v-else class="project-editor-view">
      <div class="admin-page-header">
        <div>
          <h2 class="admin-title">{{ currentProject.id ? 'Projeyi Düzenle' : 'Yeni Proje Ekle' }}</h2>
          <button @click="closeEditor" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; display: flex; align-items: center; gap: 0.5rem; margin-top: 0.5rem;">
            <i class="fas fa-arrow-left"></i> Listeye Dön
          </button>
        </div>
        <div style="display: flex; gap: 10px;">
          <button @click="clearTranslations" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
            <i class="fas fa-eraser"></i> Çeviriyi Sil
          </button>
          <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
            <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
            {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
          </button>
          <button @click="saveProject" class="admin-btn admin-btn-primary" :disabled="saving">
            <i class="fas" :class="saving ? 'fa-spinner fa-spin' : 'fa-save'"></i> 
            {{ saving ? 'Kaydediliyor...' : 'Projeyi Kaydet' }}
          </button>
        </div>
      </div>

      <div class="admin-grid-2-1">
        <div class="admin-card" style="display: flex; flex-direction: column; gap: 1.5rem;">
          <div class="admin-form-group">
            <label class="admin-label">Proje Başlığı [TR]</label>
            <input type="text" v-model="currentProject.title" class="admin-input" placeholder="Projenin Adı" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Proje Başlığı [EN]</label>
            <input type="text" v-model="currentProject.titleEn" class="admin-input" placeholder="Project Title" />
          </div>

          <div class="admin-form-group" style="position: relative;">
            <label class="admin-label">Kategori</label>
            
            <!-- Custom Dropdown Selector -->
            <div 
              class="admin-input" 
              style="display: flex; align-items: center; justify-content: space-between; cursor: pointer; user-select: none;"
              @click="showCategoryDropdown = !showCategoryDropdown"
            >
              <div style="display: flex; align-items: center; gap: 8px;">
                <template v-if="currentProject.projectCategoryId && categories.find(c => c.id === currentProject.projectCategoryId)">
                  <i v-if="categories.find(c => c.id === currentProject.projectCategoryId).icon" :class="categories.find(c => c.id === currentProject.projectCategoryId).icon"></i>
                  <span>{{ categories.find(c => c.id === currentProject.projectCategoryId).name }}</span>
                </template>
                <span v-else style="color: var(--admin-text-muted);">Kategori Seçin</span>
              </div>
              <i class="fas fa-chevron-down" style="font-size: 0.8rem; color: var(--admin-text-muted);"></i>
            </div>
            
            <!-- Dropdown Content -->
            <div v-if="showCategoryDropdown" 
                 style="position: absolute; top: calc(100% + 4px); left: 0; right: 0; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: 8px; z-index: 50; max-height: 250px; overflow-y: auto; box-shadow: 0 10px 30px rgba(0,0,0,0.5);">
              <div 
                style="padding: 12px 16px; cursor: pointer; display: flex; align-items: center; gap: 8px; transition: background 0.2s;"
                onmouseover="this.style.background='var(--admin-surface-hover)'"
                onmouseout="this.style.background='transparent'"
                @click="currentProject.projectCategoryId = null; showCategoryDropdown = false"
              >
                <span style="color: var(--admin-text-muted);">Kategori Yok (Seçimi Temizle)</span>
              </div>
              <div 
                v-for="cat in categories" :key="cat.id" 
                style="padding: 12px 16px; cursor: pointer; display: flex; align-items: center; gap: 8px; transition: background 0.2s; border-top: 1px solid rgba(255,255,255,0.05);"
                onmouseover="this.style.background='var(--admin-surface-hover)'"
                onmouseout="this.style.background='transparent'"
                @click="currentProject.projectCategoryId = cat.id; showCategoryDropdown = false"
              >
                <i v-if="cat.icon" :class="cat.icon" style="color: var(--admin-primary); font-size: 1.1rem; width: 24px; text-align: center;"></i>
                <span>{{ cat.name }}</span>
              </div>
            </div>

            <!-- Click outside overlay -->
            <div v-if="showCategoryDropdown" @click="showCategoryDropdown = false" style="position: fixed; inset: 0; z-index: 40;"></div>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Kısa Özet [TR]</label>
            <textarea v-model="currentProject.summary" class="admin-input" placeholder="Projenin kısa özeti" style="min-height: 80px;"></textarea>
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Kısa Özet [EN]</label>
            <textarea v-model="currentProject.summaryEn" class="admin-input" placeholder="Short summary" style="min-height: 80px;"></textarea>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Projenin Amacı [TR]</label>
            <textarea v-model="currentProject.aim" class="admin-input" placeholder="Bu proje ne için yapıldı?"></textarea>
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Projenin Amacı [EN]</label>
            <textarea v-model="currentProject.aimEn" class="admin-input" placeholder="Project aim"></textarea>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Zorluklar ve Çözümler [TR]</label>
            <textarea v-model="currentProject.challengesAndSolutions" class="admin-input" placeholder="Geliştirme sürecindeki zorluklar..."></textarea>
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Zorluklar ve Çözümler [EN]</label>
            <textarea v-model="currentProject.challengesAndSolutionsEn" class="admin-input" placeholder="Challenges & Solutions..."></textarea>
          </div>
        </div>

        <div style="display: flex; flex-direction: column; gap: 2rem;">
          <div class="admin-card">
            <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem; display: flex; justify-content: space-between; align-items: center;">
              <span>Medya (Görseller ve Videolar)</span>
            </h3>

            <!-- Tema Seçim Sekmeleri -->
            <div style="display: flex; gap: 1rem; margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border);">
               <button @click.prevent="mediaTab = 'general'" :style="{ outline: 'none', padding: '0.5rem 1rem', background: 'transparent', border: 'none', borderBottom: mediaTab === 'general' ? '2px solid var(--admin-primary)' : 'none', color: mediaTab === 'general' ? 'var(--admin-primary)' : 'var(--admin-text-muted)', cursor: 'pointer', fontWeight: mediaTab === 'general' ? 'bold' : 'normal' }">Genel / Varsayılan</button>
               <button @click.prevent="mediaTab = 'light'" :style="{ outline: 'none', padding: '0.5rem 1rem', background: 'transparent', border: 'none', borderBottom: mediaTab === 'light' ? '2px solid var(--admin-primary)' : 'none', color: mediaTab === 'light' ? 'var(--admin-primary)' : 'var(--admin-text-muted)', cursor: 'pointer', fontWeight: mediaTab === 'light' ? 'bold' : 'normal' }">Açık Tema (Light)</button>
               <button @click.prevent="mediaTab = 'dark'" :style="{ outline: 'none', padding: '0.5rem 1rem', background: 'transparent', border: 'none', borderBottom: mediaTab === 'dark' ? '2px solid var(--admin-primary)' : 'none', color: mediaTab === 'dark' ? 'var(--admin-primary)' : 'var(--admin-text-muted)', cursor: 'pointer', fontWeight: mediaTab === 'dark' ? 'bold' : 'normal' }">Karanlık Tema (Dark)</button>
            </div>

            <!-- VİDEO YÜKLEME ALANI -->
            <div class="admin-form-group" style="background: var(--admin-btn-secondary-bg); padding: 1rem; border-radius: 8px;">
              <label class="admin-label">{{ mediaTab === 'general' ? 'Genel Video' : (mediaTab === 'light' ? 'Açık Tema Videosu' : 'Karanlık Tema Videosu') }}</label>
              <div style="display: flex; flex-direction: column; gap: 10px;">
                <input type="text" 
                       v-model="currentProject[mediaTab === 'general' ? 'videoUrl' : (mediaTab === 'light' ? 'lightVideoUrl' : 'darkVideoUrl')]" 
                       class="admin-input" 
                       placeholder="YouTube Video Linkini buraya yapıştırabilirsiniz..." />
                
                <div style="display: flex; gap: 10px; align-items: center; margin-top: 10px;">
                  <span style="font-size: 0.9rem; color: var(--admin-text-muted);">Veya bilgisayardan yükle:</span>
                  <input type="file" @change="uploadVideoFile($event, mediaTab)" accept="video/mp4,video/webm" class="admin-input" style="padding: 0.5rem; flex: 1;" />
                  <span v-if="uploadingVideo" class="text-primary"><i class="fas fa-spinner fa-spin"></i> Yükleniyor...</span>
                </div>
                
                <div v-if="currentProject[mediaTab === 'general' ? 'videoUrl' : (mediaTab === 'light' ? 'lightVideoUrl' : 'darkVideoUrl')]" style="margin-top: 10px; padding: 10px; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: 8px;">
                  <span style="font-size: 0.85rem; color: #10b981;"><i class="fas fa-check-circle"></i> Video eklendi: {{ currentProject[mediaTab === 'general' ? 'videoUrl' : (mediaTab === 'light' ? 'lightVideoUrl' : 'darkVideoUrl')] }}</span>
                  <button @click.prevent="currentProject[mediaTab === 'general' ? 'videoUrl' : (mediaTab === 'light' ? 'lightVideoUrl' : 'darkVideoUrl')] = ''" style="margin-left: 10px; background: none; border: none; color: #ef4444; cursor: pointer; text-decoration: underline; font-size: 0.85rem;">Kaldır</button>
                </div>
              </div>
            </div>

            <!-- 3D MODEL YÜKLEME ALANI -->
            <div class="admin-form-group" style="background: rgba(255, 77, 0, 0.05); border: 1px solid rgba(255, 77, 0, 0.2); padding: 1rem; border-radius: 8px; margin-top: 1.5rem;">
              <label class="admin-label" style="color: var(--admin-primary);"><i class="fas fa-cube"></i> 3D WebGL Model (.glb / .gltf)</label>
              <div style="display: flex; flex-direction: column; gap: 10px;">
                <input type="text" 
                       v-model="currentProject.model3DUrl" 
                       class="admin-input" 
                       placeholder="3D Model Linkini buraya yapıştırabilirsiniz..." />
                
                <div style="display: flex; gap: 10px; align-items: center; margin-top: 10px;">
                  <span style="font-size: 0.9rem; color: var(--admin-text-muted);">Veya bilgisayardan yükle:</span>
                  <input type="file" @change="uploadModelFile($event)" accept=".glb,.gltf" class="admin-input" style="padding: 0.5rem; flex: 1;" />
                  <span v-if="uploadingModel" class="text-primary"><i class="fas fa-spinner fa-spin"></i> Yükleniyor...</span>
                </div>
                
                <div v-if="currentProject.model3DUrl" style="margin-top: 10px; padding: 10px; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: 8px;">
                  <span style="font-size: 0.85rem; color: #10b981;"><i class="fas fa-check-circle"></i> 3D Model eklendi: {{ currentProject.model3DUrl }}</span>
                  <button @click.prevent="currentProject.model3DUrl = ''" style="margin-left: 10px; background: none; border: none; color: #ef4444; cursor: pointer; text-decoration: underline; font-size: 0.85rem;">Kaldır</button>
                </div>
              </div>
            </div>

            <!-- GÖRSEL YÜKLEME ALANI -->
            <div style="margin-top: 2rem;">
              <label class="admin-label">{{ mediaTab === 'general' ? 'Genel Görseller' : (mediaTab === 'light' ? 'Açık Tema Görselleri' : 'Karanlık Tema Görselleri') }}</label>
              <div style="display: flex; flex-direction: column; gap: 1rem;">
                <div v-if="currentProject[mediaTab === 'general' ? 'imageUrls' : (mediaTab === 'light' ? 'lightImageUrls' : 'darkImageUrls')]?.length > 0" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(120px, 1fr)); gap: 1rem;">
                  <div v-for="(img, idx) in currentProject[mediaTab === 'general' ? 'imageUrls' : (mediaTab === 'light' ? 'lightImageUrls' : 'darkImageUrls')]" :key="idx" style="position: relative; border-radius: 8px; overflow: hidden; border: 1px solid var(--admin-border);">
                    <img :src="getFullUrl(img)" style="width: 100%; height: 120px; object-fit: cover; display: block; border-radius: 8px;" />
                    <button @click.prevent="removeImage(idx, mediaTab)" style="position: absolute; top: -6px; right: -6px; background: var(--admin-danger); color: white; border: 2px solid var(--admin-surface); width: 28px; height: 28px; border-radius: 50%; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 14px; box-shadow: 0 4px 10px rgba(239, 68, 68, 0.4); z-index: 10;">
                      <i class="ph ph-x"></i>
                    </button>
                    <div v-if="idx === 0" style="position: absolute; bottom: 4px; left: 4px; background: var(--admin-primary); color: var(--admin-heading); font-size: 0.7rem; padding: 2px 6px; border-radius: 4px;">Kapak</div>
                  </div>
                </div>
                
                <ImageUploader :modelValue="newImageUrl" @update:modelValue="(url) => handleNewImage(url, mediaTab)" label="Yeni Görsel Yükle" />
              </div>
            </div>

            <!-- MEDYA SEO AÇIKLAMALARI (ALT & ARIA) -->
            <div style="margin-top: 2rem; border-top: 1px solid var(--admin-border); padding-top: 1.5rem;">
              <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1rem;"><i class="fas fa-search"></i> Medya SEO ve Erişilebilirlik</h3>
              <div class="admin-form-group">
                <label class="admin-label">Görsel SEO Açıklaması (Image Alt Text)</label>
                <input type="text" v-model="currentProject.imageAltText" class="admin-input" placeholder="Örn: Modern ofis tasarımının önden görünüşü" />
                <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 0.5rem;">Görme engelliler ve arama motorları için resimlerin içeriğini anlatan gizli metin.</p>
              </div>
              
              <div class="admin-form-group" style="margin-top: 1rem;">
                <label class="admin-label">Video/3D Model SEO Açıklaması (Aria Label)</label>
                <input type="text" v-model="currentProject.videoAriaLabel" class="admin-input" placeholder="Örn: 3 Boyutlu Karakter Oyunu Oynanış Videosu" />
                <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 0.5rem;">Videolar ve 3D modeller için erişilebilirlik ve yapay zeka okuma etiketi.</p>
              </div>
            </div>

            <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 1rem;">Tema sekmelerini boş bırakırsanız, o tema için "Genel" sekmesindeki içerikler varsayılan olarak kullanılır.</p>
          </div>

          <div class="admin-card">
            <div class="admin-form-group">
              <label class="admin-label">Teknoloji Etiketleri</label>
              
              <!-- Mevcut Etiketler -->
              <div v-if="currentProject.techTags && currentProject.techTags.length > 0" style="display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 12px;">
                <div v-for="(tag, idx) in currentProject.techTags" :key="idx" style="background: var(--admin-btn-secondary-bg); border: 1px solid var(--admin-border); padding: 4px 10px; border-radius: 4px; display: flex; align-items: center; gap: 8px; font-size: 0.85rem;">
                  <i v-if="tag.includes('|')" :class="tag.split('|')[0]"></i>
                  <span>{{ tag.includes('|') ? tag.split('|')[1] : tag }}</span>
                  <button @click.prevent="removeTag(idx)" style="background:none; border:none; color: var(--admin-danger); cursor:pointer;"><i class="ph ph-x" style="font-size: 1.1rem; font-weight: bold;"></i></button>
                </div>
              </div>

              <!-- Yeni Etiket Ekleme Formu -->
              <div style="display: flex; gap: 8px; align-items: flex-start;">
                <div style="width: 120px;">
                  <IconPicker v-model="newTagIcon" />
                </div>
                <input type="text" v-model="newTagName" placeholder="Etiket Adı (Örn: Vue 3)" class="admin-input" style="flex: 1;" @keyup.enter="addTag" />
                <button @click.prevent="addTag" class="admin-btn admin-btn-secondary"><i class="fas fa-plus"></i></button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Category Modal -->
    <div v-if="showCategoryModal" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center;">
      <div class="admin-card" style="width: 400px; max-width: 90%;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.2rem; color: var(--admin-heading); margin: 0;">Kategori Yönetimi</h3>
          <button @click="showCategoryModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div style="margin-bottom: 1.5rem; max-height: 200px; overflow-y: auto;" class="admin-scroll">
          <ul style="list-style: none; padding: 0; margin: 0;">
            <li v-for="cat in categories" :key="cat.id" style="padding: 0.75rem; border-bottom: 1px solid var(--admin-border); display: flex; justify-content: space-between; align-items: center;">
              <span>{{ cat.name }}</span>
              <button @click="deleteCategory(cat.id)" style="background:none; border:none; color:var(--admin-danger); cursor:pointer;" title="Kategoriyi Sil">
                <i class="fas fa-trash"></i>
              </button>
            </li>
            <li v-if="categories.length === 0" style="padding: 0.75rem; color: var(--admin-text-muted); text-align: center;">Kategori bulunamadı.</li>
          </ul>
        </div>

        <form @submit.prevent="addCategory" style="display: flex; flex-direction: column; gap: 1rem;">
          <div style="display: flex; gap: 0.75rem;">
            <div style="flex: 1; min-width: 140px;">
              <IconPicker v-model="newCategoryIcon" mode="icon" />
            </div>
            <input type="text" v-model="newCategoryName" class="admin-input" placeholder="Kategori Adı [TR]" style="flex: 2; margin: 0;" required />
            <input type="text" v-model="newCategoryNameEn" class="admin-input" placeholder="Kategori Adı [EN]" style="flex: 2; margin: 0;" />
          </div>
          <button type="submit" class="admin-btn admin-btn-primary" style="justify-content: center;">Yeni Kategori Ekle</button>
        </form>
      </div>
    </div>

    <!-- Trash Modal -->
    <div v-if="showTrashModal" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center;">
      <div class="admin-card" style="width: 600px; max-width: 90%; max-height: 80vh; display: flex; flex-direction: column;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.2rem; color: var(--admin-danger); margin: 0;"><i class="fas fa-trash-alt"></i> Çöp Kutusu</h3>
          <button @click="showTrashModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div v-if="loadingTrash" style="text-align: center; padding: 2rem; color: var(--admin-primary);">
          <i class="fas fa-spinner fa-spin fa-2x"></i>
        </div>
        
        <div v-else-if="trashItems.length === 0" style="text-align: center; padding: 2rem; color: var(--admin-text-muted);">
          Çöp kutusu boş.
        </div>
        
        <div v-else style="overflow-y: auto;" class="admin-scroll">
          <ul style="list-style: none; padding: 0; margin: 0;">
            <li v-for="item in trashItems" :key="item.id" style="padding: 1rem; border-bottom: 1px solid var(--admin-border); display: flex; justify-content: space-between; align-items: center;">
              <div>
                <strong style="color: var(--admin-heading); display: block; margin-bottom: 0.25rem;">{{ item.title }}</strong>
                <span style="font-size: 0.8rem; color: var(--admin-text-muted);">{{ item.category?.name || 'Kategori Yok' }}</span>
              </div>
              <div style="display: flex; gap: 0.5rem;">
                <button @click="restoreProject(item.id)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  <i class="fas fa-undo"></i> Kurtar
                </button>
                <button @click="hardDeleteProject(item.id)" class="admin-btn" style="background: var(--admin-danger); color: white; border: none; padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  Kalıcı Sil
                </button>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import api from '@/services/api'
import translationService from '@/services/translationService'
import ImageUploader from '@/components/ImageUploader.vue'
import IconPicker from '@/components/IconPicker.vue'
import swal from '@/utils/swal'

const projects = ref([])
const categories = ref([])
const loading = ref(true)
const isEditing = ref(false)
const saving = ref(false)
const errorMsg = ref('')
const successMsg = ref('')
const showCategoryModal = ref(false)
const showCategoryDropdown = ref(false)
const newCategoryName = ref('')
const newCategoryNameEn = ref('')
const newCategoryIcon = ref('')
const mediaTab = ref('general')
const aiLoading = ref(false)

const showTrashModal = ref(false)
const trashItems = ref([])
const loadingTrash = ref(false)

const currentProject = ref({})
const rawTags = ref('') // Optional but kept for compatibility
const newTagIcon = ref('fa-brands fa-vuejs')
const newTagName = ref('')
const newImageUrl = ref('')

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const loadData = async () => {
  loading.value = true
  try {
    const [projRes, catRes] = await Promise.all([
      api.get('/Projects'),
      api.get('/Projects/categories')
    ])
    categories.value = catRes.data
    projects.value = projRes.data.map(p => {
      p.category = categories.value.find(c => c.id === p.projectCategoryId)
      return p
    })
  } catch (err) {
    errorMsg.value = 'Veriler yüklenirken bir hata oluştu.'
  } finally {
    loading.value = false
  }
}

const addCategory = async () => {
  if (!newCategoryName.value) return
  try {
    const res = await api.post('/Projects/categories', { name: newCategoryName.value, nameEn: newCategoryNameEn.value, icon: newCategoryIcon.value })
    categories.value.push(res.data)
    newCategoryName.value = ''
    newCategoryNameEn.value = ''
    newCategoryIcon.value = ''
  } catch (err) {
    alert("Kategori eklenemedi!")
  }
}

const deleteCategory = async (id) => {
  if (!confirm("Bu kategoriyi silmek istediğinize emin misiniz?")) return;
  try {
    await api.delete('/Projects/categories/' + id)
    categories.value = categories.value.filter(c => c.id !== id)
  } catch (err) {
    alert("Kategori silinirken hata oluştu.")
  }
}

const uploadingVideo = ref(false)

const uploadVideoFile = async (event, type = 'general') => {
  const file = event.target.files[0]
  if (!file) return

  const formData = new FormData()
  formData.append('file', file)

  uploadingVideo.value = true
  try {
    const response = await api.post('/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    if (response.data && response.data.url) {
      if (!currentProject.value) currentProject.value = {}
      
      if (type === 'general') currentProject.value.videoUrl = response.data.url
      else if (type === 'light') currentProject.value.lightVideoUrl = response.data.url
      else if (type === 'dark') currentProject.value.darkVideoUrl = response.data.url
      
      successMsg.value = 'Video başarıyla yüklendi.'
      setTimeout(() => { successMsg.value = '' }, 3000)
    }
  } catch (error) {
    errorMsg.value = 'Video yüklenirken hata oluştu.'
    console.error('Video upload error:', error)
  } finally {
    uploadingVideo.value = false
    event.target.value = '' // Reset input
  }
}

const uploadingModel = ref(false)

const uploadModelFile = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  const formData = new FormData()
  formData.append('file', file)

  uploadingModel.value = true
  try {
    const response = await api.post('/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    if (response.data && response.data.url) {
      if (!currentProject.value) currentProject.value = {}
      
      currentProject.value.model3DUrl = response.data.url
      
      successMsg.value = '3D Model başarıyla yüklendi.'
      setTimeout(() => { successMsg.value = '' }, 3000)
    }
  } catch (error) {
    errorMsg.value = '3D Model yüklenirken hata oluştu.'
    console.error('Model upload error:', error)
  } finally {
    uploadingModel.value = false
    event.target.value = '' // Reset input
  }
}

const openEditor = (project) => {
  mediaTab.value = 'general'
  if (project) {
    currentProject.value = { ...project }
    rawTags.value = project.techTags ? project.techTags.join(', ') : ''
  } else {
    currentProject.value = {
      id: 0,
      title: '',
      titleEn: '',
      summary: '',
      summaryEn: '',
      projectCategoryId: 0,
      aim: '',
      aimEn: '',
      challengesAndSolutions: '',
      challengesAndSolutionsEn: '',
      techTags: [],
      imageUrls: [],
      lightImageUrls: [],
      darkImageUrls: [],
      videoUrl: '',
      lightVideoUrl: '',
      darkVideoUrl: '',
      model3DUrl: '',
      imageAltText: '',
      videoAriaLabel: ''
    }
    rawTags.value = ''
    newImageUrl.value = ''
  }
  isEditing.value = true
  errorMsg.value = ''
}

const closeEditor = () => {
  isEditing.value = false
  currentProject.value = {}
}

const handleNewImage = (url, type = 'general') => {
  if(url) {
    if (type === 'general') {
      if(!currentProject.value.imageUrls) currentProject.value.imageUrls = [];
      currentProject.value.imageUrls.push(url);
    } else if (type === 'light') {
      if(!currentProject.value.lightImageUrls) currentProject.value.lightImageUrls = [];
      currentProject.value.lightImageUrls.push(url);
    } else if (type === 'dark') {
      if(!currentProject.value.darkImageUrls) currentProject.value.darkImageUrls = [];
      currentProject.value.darkImageUrls.push(url);
    }
    setTimeout(() => {
      newImageUrl.value = ''; 
    }, 50);
  }
}

const removeImage = (index, type = 'general') => {
  if (type === 'general' && currentProject.value.imageUrls) currentProject.value.imageUrls.splice(index, 1);
  if (type === 'light' && currentProject.value.lightImageUrls) currentProject.value.lightImageUrls.splice(index, 1);
  if (type === 'dark' && currentProject.value.darkImageUrls) currentProject.value.darkImageUrls.splice(index, 1);
}

const addTag = () => {
  if (!newTagName.value) return
  if (!currentProject.value.techTags) currentProject.value.techTags = []
  
  const tagString = newTagIcon.value ? `${newTagIcon.value}|${newTagName.value}` : newTagName.value
  currentProject.value.techTags.push(tagString)
  
  newTagName.value = ''
  // Keep icon same for quick adding multiple
}

const removeTag = (index) => {
  currentProject.value.techTags.splice(index, 1)
}

const updateTags = () => {
  // kept for compatibility if needed
}

// Removed watch on primaryImage

const translateWithAI = async () => {
  aiLoading.value = true;
  swal.fire({
    title: 'Yapay Zeka Çeviriyor...',
    html: 'Proje detayları İngilizceye çevriliyor. Lütfen bekleyin...',
    allowOutsideClick: false,
    didOpen: () => {
      swal.showLoading();
    }
  });

  try {
    if (currentProject.value.title && !currentProject.value.titleEn) {
      const res = await translationService.translate(currentProject.value.title, 'English', 'Project');
      currentProject.value.titleEn = res?.translatedText || currentProject.value.titleEn;
    }
    if (currentProject.value.summary && !currentProject.value.summaryEn) {
      const res = await translationService.translate(currentProject.value.summary, 'English', 'Project');
      currentProject.value.summaryEn = res?.translatedText || currentProject.value.summaryEn;
    }
    if (currentProject.value.aim && !currentProject.value.aimEn) {
      const res = await translationService.translate(currentProject.value.aim, 'English', 'Project');
      currentProject.value.aimEn = res?.translatedText || currentProject.value.aimEn;
    }
    if (currentProject.value.challengesAndSolutions && !currentProject.value.challengesAndSolutionsEn) {
      const res = await translationService.translate(currentProject.value.challengesAndSolutions, 'English', 'Project');
      currentProject.value.challengesAndSolutionsEn = res?.translatedText || currentProject.value.challengesAndSolutionsEn;
    }
    
    // Kategorileri çevirme
    for (let cat of categories.value) {
      if (cat.name && !cat.nameEn) {
        const res = await translationService.translate(cat.name, 'English', 'Project');
        cat.nameEn = res?.translatedText || cat.nameEn;
        await api.put(`/Projects/categories/${cat.id}`, cat); 
      }
    }
    
    if (newCategoryName.value && !newCategoryNameEn.value) {
        const res = await translationService.translate(newCategoryName.value, 'English', 'Project');
        newCategoryNameEn.value = res?.translatedText || newCategoryNameEn.value;
    }

    successMsg.value = 'Metinler başarıyla İngilizceye çevrildi!';
    swal.fire('Başarılı', 'Proje detayları başarıyla İngilizceye çevrildi!', 'success');
  } catch (err) {
    swal.fire('Hata', 'Çeviri sırasında bir hata oluştu.', 'error');
  } finally {
    aiLoading.value = false;
  }
}

const clearTranslations = async () => {
  const confirm = await swal.fire({
    title: 'Çevirileri Temizle?',
    text: 'Bu formdaki (Başlık, Özet, Açıklama) tüm İngilizce çeviriler silinecektir. Emin misiniz?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Evet, Temizle',
    cancelButtonText: 'İptal'
  });

  if (confirm.isConfirmed) {
    currentProject.value.titleEn = '';
    currentProject.value.shortDescriptionEn = '';
    currentProject.value.fullDescriptionEn = '';
    
    swal.fire('Temizlendi', 'İngilizce alanlar boşaltıldı. Kaydet butonuna basarak değişiklikleri kaydedebilirsiniz.', 'info');
  }
}

const saveProject = async () => {
  if(currentProject.value.projectCategoryId === 0) {
    errorMsg.value = "Lütfen bir kategori seçin."
    return
  }

  saving.value = true
  errorMsg.value = ''
  
  try {
    if (currentProject.value.id > 0) {
      await api.put(`/Projects/${currentProject.value.id}`, currentProject.value)
      successMsg.value = 'Proje güncellendi!'
    } else {
      await api.post('/Projects', currentProject.value)
      successMsg.value = 'Proje eklendi!'
    }
    
    setTimeout(() => { successMsg.value = '' }, 3000)
    isEditing.value = false
    await loadData()
  } catch (err) {
    errorMsg.value = 'Proje kaydedilirken hata oluştu: ' + (err.response?.data || err.message)
  } finally {
    saving.value = false
  }
}

const deleteProject = async (id) => {
  try {
    const confirm = await swal.fire({
      title: 'Emin misiniz?',
      text: 'Bu proje Çöp Kutusuna taşınacak.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Evet, Çöpe At!',
      cancelButtonText: 'İptal'
    })

    if (confirm.isConfirmed) {
      await api.delete('/Projects/' + id)
      successMsg.value = 'Proje çöp kutusuna taşındı.'
    }
  } catch (error) {
    if (error.response && error.response.status === 404) {
      successMsg.value = 'Proje zaten silinmiş.'
    } else {
      errorMsg.value = 'Proje silinirken hata oluştu.'
      console.error('Delete error:', error)
    }
  } finally {
    loadData()
  }
}

const openTrash = async () => {
  showTrashModal.value = true
  await loadTrash()
}

const loadTrash = async () => {
  loadingTrash.value = true
  try {
    const res = await api.get('/Projects/trash')
    trashItems.value = res.data
  } catch (error) {
    console.error("Çöp kutusu yüklenemedi", error)
  } finally {
    loadingTrash.value = false
  }
}

const restoreProject = async (id) => {
  try {
    await api.post(`/Projects/${id}/restore`)
    successMsg.value = 'Proje başarıyla kurtarıldı.'
    await loadTrash()
    await loadData()
    if(trashItems.value.length === 0) showTrashModal.value = false
  } catch (error) {
    alert("Kurtarma işlemi başarısız.")
  }
}

const hardDeleteProject = async (id) => {
  const confirm = await swal.fire({
    title: 'Kalıcı Olarak Sil?',
    text: 'Bu işlem geri alınamaz!',
    icon: 'error',
    showCancelButton: true,
    confirmButtonText: 'Evet, Tamamen Sil',
    cancelButtonText: 'İptal'
  })

  if (confirm.isConfirmed) {
    try {
      await api.delete(`/Projects/${id}/hard`)
      successMsg.value = 'Proje kalıcı olarak silindi.'
      await loadTrash()
      if(trashItems.value.length === 0) showTrashModal.value = false
    } catch (error) {
      alert("Kalıcı silme işlemi başarısız.")
    }
  }
}

onMounted(() => {
  loadData()
})
</script>
